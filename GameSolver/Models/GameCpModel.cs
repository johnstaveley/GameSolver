using Google.OrTools.Sat;

namespace GameSolver.Models
{
    public class GameCpModel : CpModel
    {
        /// <summary>
        /// These are used to set the column constraints
        /// </summary>
        public List<List<IntVar>> Columns { get; set; } = [];
        /// <summary>
        /// These are used to hold the row constraints
        /// </summary>
        public List<List<IntVar>> Rows { get; set; } = [];
        /// <summary>
        /// The initial values and solution
        /// </summary>
        public IntVar[,] Cells { get; set; } = new IntVar[0, 0];
    }
}
