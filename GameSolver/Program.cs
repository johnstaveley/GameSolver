using GameSolver.Services;
using GameSolver.Models;

// Puzzles taken from the British Army challenge book 2019

var sudokuNonConsecutiveGame = new Game
{
    FileName = "SudokuNonConsec.txt",
    SubGridSize = new Tuple<int, int>(3, 3),
    IsNonConsecutive = true,
    IsDebug = true
};
var outputLines = GameProcessor.Process(sudokuNonConsecutiveGame);
var expectedFirstLine = "746318592";
if (outputLines.First() != expectedFirstLine)
{
    Console.WriteLine($"Sudoku Test Failed, expected {expectedFirstLine}, but got {outputLines.First()}");
}
else
{
    Console.WriteLine("Sudoku Test Passed expected result");
}
Console.ReadKey();
