using GameSolver.Models;
using GameSolver.Utility;
using System.Text.RegularExpressions;

namespace GameSolver.Services
{
    public static class GameProcessor
    {
        public static string[] Process(Game game)

        {
            string filePath = $"Problems\\{game.FileName}";
            string[] lines = File.ReadAllLines(filePath);
            if (game.IsDebug) { 
                Console.WriteLine($"Read {lines.Length} lines from {filePath}");
            }
            List<string> inputData = new List<string>();
            // Strip out constraints
            string approvedPattern = @"[^0-9]";
            foreach (var line in lines)
            {
                inputData.Add(Regex.Replace(line, approvedPattern, ""));
            }
            var map = new Map(inputData.Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 5).ToArray(), game.IsDebug);
            if (game.IsDebug)
            {
                map.Display();
            }
            var constraints = new ConstraintsService(lines, game.IsDebug);
            var solver = new GameSolverService(map, constraints, game);

            var solution = solver.Solve();
            Solution.PrintSolution(solution);
            map.ValidateSolution(solution, game.IsDebug);
            var outputLines = new List<string>();
            var outputLine = "";
            for (int i = 0; i < solution.GetLength(1); i++)
            {
                outputLine += solution[i, 0].ToString();
            }
            outputLines.Add(outputLine);
            return outputLines.ToArray();
        }
    }
}
