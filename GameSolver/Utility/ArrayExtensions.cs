namespace GameSolver.Utility
{
    public static class ArrayExtensions
    {
        public static ushort?[,] GetGrid(string[] lines, bool isDebug)
        {
            var rows = lines.Length;
            var cols = lines.Max(l => l.Length);
            ushort?[,] grid = new ushort?[rows, cols];

            // Populate the 2D grid
            for (int y = 0; y < rows; y++)
            {
                char[] elements = lines[y].ToCharArray();
                for (int x = 0; x < cols; x++)
                {
                    var value = elements[x].ToString();
                    grid[x, y] = value == "0" ? (ushort?)null : (ushort)int.Parse(value);
                }
            }
            if (isDebug)
            {
                Console.WriteLine($"Grid loaded of size {grid.GetLength(0)}:{grid.GetLength(1)}");
            }
            return grid;
        }
    }
}
