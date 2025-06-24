
using GameSolver.Utility;

namespace GameSolver.Models
{
    public class Map
    {        
        public ushort?[,] Grid { get; set; }
        public Map(string[] lines, bool isDebug)
        {
            Grid = ArrayExtensions.GetGrid(lines, isDebug);
        }
        public void Display()
        {
            Console.WriteLine("Initial Values:");
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                for (int x = 0; x < Grid.GetLength(0); x++)
                {
                    Console.Write(Grid[x, y]?.ToString() ?? "0");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void ValidateSolution(ushort?[,] solution, bool isDebug)
        {
            if (solution.GetLength(0) != Grid.GetLength(0) || solution.GetLength(1) != Grid.GetLength(1))
            {
                throw new ArgumentException("Solution grid size does not match the original grid size.");
            }
            var isPassed = true;
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                for (int x = 0; x < Grid.GetLength(0); x++)
                {
                    if (Grid[x, y] != null && solution[x, y] != Grid[x, y])
                    {
                        isPassed = false;
                    }
                }
            }
            if (!isPassed)
            {
                throw new InvalidOperationException("Solution does not match the initial values.");
            }
            if (isDebug)
            {
                Console.WriteLine("Solution validated successfully against initial values.");
            }
        }
    }
}
