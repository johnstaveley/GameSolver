namespace GameSolver.Models
{
    public class Game
    {
        /// <summary>
        /// Print additional debug statements
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// Neighbouring squares never contain consecutive values such as 3 and 4
        /// </summary>
        public bool IsNonConsecutive { get; set; }
        /// <summary>
        /// File name of the input values and constraints
        /// </summary>
        public string FileName { get; set; } = "";
        /// <summary>
        /// The maximum value which can be entered into a cell
        /// </summary>
        public int MaxValue { get; set; } = 9;
        public Tuple<int, int>? SubGridSize { get; set; } = null;
        /// <summary>
        /// Prohibit adjacent squares from summing to 5 or 10 unless explicitly allowed by the constraints.
        /// </summary>
        public bool ProhibitXV { get; set; } = false;
        public string Description { get; set; } = "No description provided";
    }
}
