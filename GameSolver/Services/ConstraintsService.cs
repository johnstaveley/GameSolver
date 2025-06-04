using GameSolver.Models;

namespace GameSolver.Services
{
    public class ConstraintsService
    {
        public List<Constraint> Constraints { get; set; }

        public ConstraintsService(string[] lines, bool isDebug)
        {
            var rows = lines.Length;
            var cols = lines.Length;
            Constraints = new List<Constraint>();
            for (int y = 0; y < rows; y++)
            {
                char[] elements = lines[y].ToCharArray();
                for (int x = 0; x < cols; x++)
                {
                    var value = elements[x].ToString();
                    switch (value)
                    {
                        case ">":
                            // Left cell is greater than Right cell
                            var leftX1 = (x - 1) / 2;
                            var rightX1 = (x + 1) / 2;
                            var leftY1 = y / 2;
                            var rightY1 = y / 2;
                            Constraints.Add(new Constraint(leftX1, leftY1, rightX1, rightY1, ConstraintType.GreaterThan));
                            break;
                        case "V":
                            // Top cell is greater than bottom cell
                            var leftX2 = x / 2;
                            var rightX2 = x / 2;
                            var leftY2 = (y - 1) / 2;
                            var rightY2 = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX2, leftY2, rightX2, rightY2, ConstraintType.GreaterThan));
                            break;
                        case "^":
                            // Top cell is lower than bottom cell
                            var leftX3 = x / 2;
                            var rightX3 = x / 2;
                            var leftY3 = (y - 1) / 2;
                            var rightY3 = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX3, leftY3, rightX3, rightY3, ConstraintType.LessThan));
                            break;
                        case "<":
                            // Left cell is less than right cell
                            var leftX4 = (x - 1) / 2;
                            var rightX4 = (x + 1) / 2;
                            var leftY4 = y / 2;
                            var rightY4 = y / 2;
                            Constraints.Add(new Constraint(leftX4, leftY4, rightX4, rightY4, ConstraintType.LessThan));
                            break;
                        case "A":
                            // 3 Cells Top left are less than the target cell
                            var leftX5 = (x - 1) / 2;
                            var leftY5 = (y - 1) / 2;
                            var rightX5 = (x + 1) / 2;
                            var rightY5 = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX5, leftY5, rightX5, rightY5, ConstraintType.LessThan));
                            Constraints.Add(new Constraint(leftX5+1, leftY5, rightX5, rightY5, ConstraintType.LessThan));
                            Constraints.Add(new Constraint(leftX5, leftY5+1, rightX5, rightY5, ConstraintType.LessThan));
                            break;
                        case "B":
                            // 3 Cells Top right are less than the target cell
                            var leftX6 = (x - 1) / 2;
                            var leftY6 = (y + 1) / 2;
                            var rightX6 = (x + 1) / 2;
                            var rightY6 = (y - 1) / 2;
                            Constraints.Add(new Constraint(leftX6, leftY6, rightX6, rightY6, ConstraintType.GreaterThan));
                            Constraints.Add(new Constraint(leftX6, leftY6, rightX6-1, rightY6, ConstraintType.GreaterThan));
                            Constraints.Add(new Constraint(leftX6, leftY6, rightX6, rightY6+1, ConstraintType.GreaterThan));
                            break;
                        case "C":
                            // 3 Cells bottom left are less than the target cell
                            var leftX7 = (x - 1) / 2;
                            var leftY7 = (y + 1) / 2;
                            var rightX7 = (x + 1) / 2;
                            var rightY7 = (y - 1) / 2;
                            Constraints.Add(new Constraint(leftX7, leftY7, rightX7, rightY7, ConstraintType.LessThan));
                            Constraints.Add(new Constraint(leftX7 + 1, leftY7, rightX7, rightY7, ConstraintType.LessThan));
                            Constraints.Add(new Constraint(leftX7, leftY7 - 1, rightX7, rightY7, ConstraintType.LessThan));
                            break;
                        case "D":
                            // 3 Cells bottom right are less than the target cell
                            var leftX8 = (x - 1) / 2;
                            var leftY8 = (y - 1) / 2;
                            var rightX8 = (x + 1) / 2;
                            var rightY8 = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX8, leftY8, rightX8, rightY8, ConstraintType.GreaterThan));
                            Constraints.Add(new Constraint(leftX8, leftY8, rightX8-1, rightY8, ConstraintType.GreaterThan));
                            Constraints.Add(new Constraint(leftX8, leftY8, rightX8, rightY8-1, ConstraintType.GreaterThan));
                            break;
                        case "E":
                            // Top left are greater than the target cell
                            var leftX5A = (x - 1) / 2;
                            var leftY5A = (y - 1) / 2;
                            var rightX5A = (x + 1) / 2;
                            var rightY5A = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX5A, leftY5A, rightX5A, rightY5A, ConstraintType.GreaterThan));
                            break;
                        case "F":
                            // Top right are greater than the target cell
                            var leftX6A = (x - 1) / 2;
                            var leftY6A = (y + 1) / 2;
                            var rightX6A = (x + 1) / 2;
                            var rightY6A = (y - 1) / 2;
                            Constraints.Add(new Constraint(leftX6A, leftY6A, rightX6A, rightY6A, ConstraintType.LessThan));
                            break;
                        case "G":
                            // Bottom left are greater than the target cell
                            var leftX7A = (x - 1) / 2;
                            var leftY7A = (y + 1) / 2;
                            var rightX7A = (x + 1) / 2;
                            var rightY7A = (y - 1) / 2;
                            Constraints.Add(new Constraint(leftX7A, leftY7A, rightX7A, rightY7A, ConstraintType.GreaterThan));
                            break;
                        case "H":
                            // Bottom right are greater than the target cell
                            var leftX8A = (x - 1) / 2;
                            var leftY8A = (y - 1) / 2;
                            var rightX8A = (x + 1) / 2;
                            var rightY8A = (y + 1) / 2;
                            Constraints.Add(new Constraint(leftX8A, leftY8A, rightX8A, rightY8A, ConstraintType.LessThan));
                            break;
                        case "x":
                            // Cells add up to 10
                            Constraints.Add(GetConstraintFromPosition(x, y, ConstraintType.EqualTo10));
                            break;
                        case "v":
                            // Cells add up to 5
                            Constraints.Add(GetConstraintFromPosition(x, y, ConstraintType.EqualTo5));
                            break;
                        case "c":
                            // Cells must be consecutive
                            Constraints.Add(GetConstraintFromPosition(x, y, ConstraintType.Consecutive));
                            break;
                        case " ":
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            break;
                        default:
                            throw new Exception($"Unknown constraint character '{value}' at position ({x}, {y})");
                    }
                }
            }
            if (isDebug)
            {
                Console.WriteLine($"Found {Constraints.Count} constraints");
                foreach (var constraint in Constraints)
                {
                    Console.WriteLine(constraint.ToString());
                }
            }
        }
        public Constraint GetConstraintFromPosition(int x, int y, ConstraintType constraintType)
        {
            var leftX = x / 2;
            var rightX = x / 2;
            var leftY = y / 2;
            var rightY = y / 2;
            var isOddY = y % 2 != 0;
            if (isOddY)
            {
                // If y is odd then it is between data rows and the relationship is top to bottom
                leftY = (y - 1) / 2;
                rightY = (y + 1) / 2;
            }
            else
            {
                // If y is even then it is in a data row and the relationship is left to right
                leftX = (x - 1) / 2;
                rightX = (x + 1) / 2;
            }
            return new Constraint(leftX, leftY, rightX, rightY, constraintType);
        }
    }
}