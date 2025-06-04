using GameSolver.Exceptions;
using GameSolver.Models;
using Google.OrTools.Sat;

namespace GameSolver.Services
{
    public class GameSolverService
    {
        private readonly Map _map;
        private readonly Game _game;
        private readonly ConstraintsService _constraints;
        private int _numberOfRows;
        private int _numberOfColumns;
        public GameSolverService(Map map, ConstraintsService constraints, Game game)
        {
            _map = map;
            _game = game;
            _constraints = constraints;
            _numberOfRows = _map.Grid.GetLength(0);
            _numberOfColumns = _map.Grid.GetLength(1);
        }

        public ushort?[,] Solve()
        {
            var model = new GameCpModel();
            model.Cells = new IntVar[_numberOfRows, _numberOfColumns];

            InitialiseVars(model);
            AddKnownValues(model);
            AddConstraints(model);

            return GetSolution(model);
        }

        private ushort?[,] GetSolution(GameCpModel model)
        {
            var solver = new CpSolver();
            var result = solver.Solve(model);

            if (result == CpSolverStatus.Optimal)
            {
                for (var columnIndex = 0; columnIndex < _numberOfColumns; columnIndex++)
                {
                    for (var rowIndex = 0; rowIndex < _numberOfRows; rowIndex++)
                    {
                        _map.Grid[columnIndex, rowIndex] = (ushort)solver.Value(model.Cells[rowIndex, columnIndex]);
                    }
                }
                return _map.Grid;
            }

            throw OptimisationException.NoSolution();
        }

        private void AddConstraints(GameCpModel model)
        {
            foreach (var row in model.Rows)
            {
                model.AddAllDifferent(row);
            }
            foreach (var column in model.Columns)
            {
                model.AddAllDifferent(column);
            }
            if (_game.IsNonConsecutive)
            {
                for (var columnIndex = 0; columnIndex < _numberOfColumns - 1; columnIndex++)
                {
                    for (var rowIndex = 0; rowIndex < _numberOfRows - 1; rowIndex++)
                    {
                        model.Add(model.Cells[rowIndex, columnIndex + 1] - model.Cells[rowIndex, columnIndex] != 1);
                        model.Add(model.Cells[rowIndex + 1, columnIndex] - model.Cells[rowIndex, columnIndex] != 1);
                        model.Add(model.Cells[rowIndex, columnIndex + 1] - model.Cells[rowIndex, columnIndex] != -1);
                        model.Add(model.Cells[rowIndex + 1, columnIndex] - model.Cells[rowIndex, columnIndex] != -1);
                    }
                }
            }
            if (_game.ProhibitXV) {
                var equalityConstraints = _constraints.Constraints
                    .Where(c => c.Type == ConstraintType.EqualTo5 || c.Type == ConstraintType.EqualTo10)
                    .ToList();
                for (var columnIndex = 0; columnIndex < _numberOfColumns - 1; columnIndex++)
                {
                    for (var rowIndex = 0; rowIndex < _numberOfRows - 1; rowIndex++)
                    {
                        // Get constraints for that cell
                        var constraintsForCell = equalityConstraints
                            .Where(c => c.LeftX == columnIndex && c.LeftY == rowIndex)
                            .ToList();
                        var hasLeftRightConstraint = constraintsForCell.Exists(a => a.RightX == columnIndex + 1 && a.RightY == rowIndex);
                        var hasUpDownConstraint = constraintsForCell.Exists(a => a.RightX == columnIndex && a.RightY == rowIndex + 1);
                        // Set left/right constraint
                        if (!hasLeftRightConstraint)
                        {
                            model.Add(model.Cells[rowIndex, columnIndex] + model.Cells[rowIndex, columnIndex + 1] != 5);
                            model.Add(model.Cells[rowIndex, columnIndex] + model.Cells[rowIndex, columnIndex + 1] != 10);
                        }
                        // Set up/down constraint
                        if (!hasUpDownConstraint)
                        {
                            model.Add(model.Cells[rowIndex, columnIndex] + model.Cells[rowIndex + 1, columnIndex] != 5);
                            model.Add(model.Cells[rowIndex, columnIndex] + model.Cells[rowIndex + 1, columnIndex] != 10);
                        }
                    }
                }
            }
            foreach (var constraint in _constraints.Constraints)
            {
                switch (constraint.Type)
                {
                    case ConstraintType.LessThan:
                        model.Add(model.Cells[constraint.LeftY, constraint.LeftX] < model.Cells[constraint.RightY, constraint.RightX]);
                        break;
                    case ConstraintType.GreaterThan:
                        model.Add(model.Cells[constraint.LeftY, constraint.LeftX] > model.Cells[constraint.RightY, constraint.RightX]);
                        break;
                    case ConstraintType.EqualTo5:
                        model.Add(model.Cells[constraint.LeftY, constraint.LeftX] + model.Cells[constraint.RightY, constraint.RightX] == 5);
                        break;
                    case ConstraintType.EqualTo10:
                        model.Add(model.Cells[constraint.LeftY, constraint.LeftX] + model.Cells[constraint.RightY, constraint.RightX] == 10);
                        break;
                    case ConstraintType.Consecutive:
                        // For a path of consecutive numbers, we need to ensure:
                        // 1. The difference between adjacent cells is exactly 1
                        // 2. The direction (increasing or decreasing) is consistent
                        var diff = model.NewIntVar(-1, 1, $"diff_{constraint.LeftX}_{constraint.LeftY}_{constraint.RightX}_{constraint.RightY}");
                        model.Add(model.Cells[constraint.RightY, constraint.RightX] - model.Cells[constraint.LeftY, constraint.LeftX] == diff);
                        
                        // Find all constraints that form a path with this one
                        var pathConstraints = _constraints.Constraints
                            .Where(c => c.Type == ConstraintType.Consecutive)
                            .Where(c => (c.LeftX == constraint.LeftX && c.LeftY == constraint.LeftY) || 
                                      (c.RightX == constraint.LeftX && c.RightY == constraint.LeftY) ||
                                      (c.LeftX == constraint.RightX && c.LeftY == constraint.RightY) ||
                                      (c.RightX == constraint.RightX && c.RightY == constraint.RightY))
                            .ToList();

                        // For each connected constraint in the path, ensure they use the same direction
                        foreach (var connectedConstraint in pathConstraints)
                        {
                            if (connectedConstraint != constraint)
                            {
                                var connectedDiff = model.NewIntVar(-1, 1, $"diff_{connectedConstraint.LeftX}_{connectedConstraint.LeftY}_{connectedConstraint.RightX}_{connectedConstraint.RightY}");
                                model.Add(model.Cells[connectedConstraint.RightY, connectedConstraint.RightX] - 
                                        model.Cells[connectedConstraint.LeftY, connectedConstraint.LeftX] == connectedDiff);
                                model.Add(connectedDiff == diff); // Ensure same direction
                            }
                        }
                        break;
                }
            }
            if (_game.SubGridSize != null)
            {
                var numberOfSubGridsY = model.Cells.GetLength(0) / _game.SubGridSize.Item1;
                var numberOfSubGridsX = model.Cells.GetLength(1) / _game.SubGridSize.Item2;
                foreach (var rowSquareIndex in Enumerable.Range(0, numberOfSubGridsY))
                {
                    foreach (var columnRowIndex in Enumerable.Range(0, numberOfSubGridsX))
                    {
                        var startRow = rowSquareIndex * _game.SubGridSize.Item1;
                        var startColumn = columnRowIndex * _game.SubGridSize.Item2;
                        List<IntVar> cells = new List<IntVar>();
                        for (var i = 0; i < _game.SubGridSize.Item1; i++)
                        {
                            for (var j = 0; j < _game.SubGridSize.Item2; j++)
                            {
                                cells.Add(model.Cells[startRow + i, startColumn + j]);
                            }
                        }
                        model.AddAllDifferent(cells);
                    }
                }
            }
        }

        private void AddKnownValues(GameCpModel model)
        {
            for (var columnIndex = 0; columnIndex < _map.Grid.GetLength(1); columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < _map.Grid.GetLength(0); rowIndex++)
                {
                    var knownValue = _map.Grid[columnIndex, rowIndex];
                    if (knownValue != null)
                    {
                        model.Add(model.Cells[rowIndex, columnIndex] == (long)knownValue);
                    }
                }
            }
        }

        private void InitialiseVars(GameCpModel model)
        {
            for (var columnIndex = 0; columnIndex < _numberOfColumns; columnIndex++)
            {
                List<IntVar> row = [];
                for (var rowIndex = 0; rowIndex < _numberOfRows; rowIndex++)
                {
                    var intVar = model.NewIntVar(1, _game.MaxValue, $"cell_{columnIndex}_{rowIndex}");
                    model.Cells[rowIndex, columnIndex] = intVar;
                    row.Add(intVar);
                }
                model.Rows.Add(row);
            }
            for (var columnIndex = 0; columnIndex < _numberOfColumns; columnIndex++)
            {
                model.Columns.Add(model.Rows.Select(r => r.ElementAt(columnIndex)).ToList());
            }
        }
    }
}
