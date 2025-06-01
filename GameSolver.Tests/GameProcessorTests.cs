using GameSolver.Models;
using GameSolver.Services;

namespace GameSolver.Tests
{
    // Puzzles taken from the British Army challenge book 2019
    public class GameProcessorTests
    {
        [Test]
        public void FutoshikiBasic()
        {
            // Arrange
            var game = new Game
            {
                Description = "Futoshiki Basic - 6x6 grid with minimal inequality constraints",
                FileName = "FutoshikiBasic.txt",
                MaxValue = 6
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("412563"));
        }
        [Test]
        public void FutoshikiFull()
        {
            // Arrange
            var game = new Game
            {
                Description = "Futoshiki Full - 6x6 grid with full inequality constraints",
                FileName = "FutoshikiFull.txt",
                MaxValue = 6
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("423165"));
        }

        [Test]
        public void SudokuInequality1()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku with inequality constraints 1",
                FileName = "SudokuInequality1.txt",
                MaxValue = 6,
                SubGridSize = new Tuple<int, int>(2, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("245163"));
        }
        [Test]
        public void SudokuInequality2()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku with inequality constraints 1",
                FileName = "SudokuInequality2.txt",
                MaxValue = 6,
                SubGridSize = new Tuple<int, int>(2, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("264531"));
        }
        [Test]
        public void SudokuEasy()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku Easy Level",
                FileName = "SudokuEasy.txt",
                SubGridSize = new Tuple<int, int>(3, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("913427856"));
        }
        [Test]
        public void SudokuMaster()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku Master Level",
                FileName = "SudokuMaster.txt",
                SubGridSize = new Tuple<int, int>(3, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("872594613"));
        }
        [Test]
        public void SudokuExtreme()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku Extreme",
                FileName = "SudokuExtreme.txt",
                SubGridSize = new Tuple<int, int>(3, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("984531672"));
        }
        [Test]
        public void SudokuQuadMax6by6()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku with indicators that each of the 3 cells at corners are less than the central cell A = Top left, B = Top Right, C = Bottom Left, D = Bottom Right",
                FileName = "SudokuQuadMax66.txt",
                MaxValue = 6,
                SubGridSize = new Tuple<int, int>(2, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("513624"));
        }
        [Test]
        public void SudokuQuadMax9by9()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku with indicators that each of the 3 cells at corners are less than the central cell A = Top left, B = Top Right, C = Bottom Left, D = Bottom Right",
                FileName = "SudokuQuadMax99.txt",
                SubGridSize = new Tuple<int, int>(3, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("871956423"));
        }
        [Test]
        public void SudokuXV()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku - Adjacent squares either must add up to 10 or 5 (see grid) or otherwise must not.",
                FileName = "SudokuXV.txt",
                SubGridSize = new Tuple<int, int>(3, 3),
                ProhibitXV = true
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("693125784"));
        }
        [Test]
        public void SudokuAntiXV()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku - No adjacent squares are allowed to add up to 10 or 5",
                FileName = "SudokuAntiXV.txt",
                SubGridSize = new Tuple<int, int>(3, 3),
                ProhibitXV = true
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("186397425"));
        }
        [Test]
        public void SudokuNonconsecutive()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku - No adjacent squares are allowed to be consecutive numbers",
                FileName = "SudokuNonConsec.txt",
                SubGridSize = new Tuple<int, int>(3, 3),
                IsNonConsecutive = true
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            Assert.That(result.First(), Is.EqualTo("746318592"));
        }
        [Test]
        public void SudokuThermometer()
        {
            // Arrange
            var game = new Game
            {
                Description = "Sudoku - Numbers must increase along the line of the 'thermometer'",
                FileName = "SudokuThermometer.txt",
                SubGridSize = new Tuple<int, int>(3, 3)
            };

            //Act
            var result = GameProcessor.Process(game);

            // Assert
            var actualSolution = new Map(result, game.IsDebug);
            var expectedSolution = LoadSolution(game);
            expectedSolution.ValidateSolution(actualSolution.Grid, game.IsDebug);
            Assert.That(result.First(), Is.EqualTo("278564139"));
        }
        private Map LoadSolution(Game game)
        {
            string filePath = $"Solutions\\{game.FileName}";
            string[] lines = File.ReadAllLines(filePath);
            return new Map(lines, false);

        }
    }
}
