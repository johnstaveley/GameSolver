using GameSolver.Exceptions;
using GameSolver.Models;
using Google.OrTools.Sat;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using System;

namespace GameSolver.Services
{
    public class GameSolverService
    {
        private readonly Map _map;
        private readonly Game _game;
        private readonly ConstraintsService _constraintService;
        private int _numberOfRows;
        private int _numberOfColumns;
        public GameSolverService(Map map, ConstraintsService constraintsService, Game game)
        {
            _map = map;
            _game = game;
            _constraintService = constraintsService;
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
                var equalityConstraints = _constraintService.Constraints
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
            // For each complete "ascending/descending sequence", you need to create a new Boolean variable x that will control whether the sequence is ascending (if true)
            // or descending(if false).Make your "ascending" constraints and use the "only enforce if" function on each one, passing in that new Boolean variable so that
            // x => (a < b); x => (b < c); etc..Then, make your "descending" constraints the same way, except pass in the negation of that same Boolean variable using the
            // "not" function and flip the comparisons so that!x => (a > b); !x => (b > c); etc..Now, either your ascending constraints are "active" or your descending
            // constraint are "active". The function names are different depending on the language you are using; check the documentation for specifics. You must use a
            // new Boolean variable for each "sequence" that you want consistent, and you must use the same Boolean variable for each constraint in that "sequence".
            var snakeId = 0;
            model.Snakes = new BoolVar[_game.Snakes.Count];
            foreach (var snake in _game.Snakes)
                {
                model.Snakes[snakeId] = model.NewBoolVar($"snake_{snakeId}");
                foreach (var constraint in snake)
                    {
                    switch (constraint.Type)
                    {
                        case ConstraintType.Snake:
                            // Snake constraints are always ascending
                            model.Add(model.Cells[constraint.LeftY, constraint.LeftX] < model.Cells[constraint.RightY, constraint.RightX])
                                .OnlyEnforceIf(model.Snakes[snakeId].Not());
                            // Snake constraints are always descending
                            model.Add(model.Cells[constraint.LeftY, constraint.LeftX] > model.Cells[constraint.RightY, constraint.RightX])
                                .OnlyEnforceIf(model.Snakes[snakeId]);
                            break;
                        default:
                            throw new ArgumentException($"Unknown snake constraint type: {constraint.Type}");
                    }
                }
                snakeId++;
            }
            foreach (var constraint in _constraintService.Constraints)
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
