namespace GameSolver.Utility
{
    public static class Solution
    {
        public static void PrintSolution<T>(T[,] solution)
        {
            if (solution == null || solution.GetLength(0) == 0 || solution.GetLength(1) == 0)
            {
                Console.WriteLine("No solution found.");
                return;
            }
            Console.WriteLine("Solution found:");
            for (int j = 0; j < solution.GetLength(1); j++)
            {
                for (int i = 0; i < solution.GetLength(0); i++)
                {
                    Console.Write(solution[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
