using System.Diagnostics;

namespace MySudoku
{
    /// <summary>
    /// Sudoku Technique interface.
    /// </summary>
    public abstract class SudokuTechnique
    {
        /// <summary>
        /// Keep track of the times the technique has been used.
        /// </summary>
        private int TimesUsed { get; set; } = 0;

        /// <summary>
        /// The cost of this technique on its first use.
        /// </summary>
        protected abstract int FirstUseCost { get; }

        /// <summary>
        /// The cost of this technique on its subsequent uses.
        /// </summary>
        protected abstract int SubsequentUseCost { get; }

        /// <summary>
        /// Should this technique log its process on the console.
        /// </summary>
        public bool LogConsole { get; set; } = false;

        /// <summary>
        /// Resets the usage count for this technique.
        /// </summary>
        public void ResetUsageCount() => TimesUsed = 0;

        /// <summary>
        /// Increments the usage count for this technique and returns the use cost based on it.
        /// </summary>
        public int GetUsageCost() => ++TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;

        /// <summary>
        /// Checks if the technique can be applied.
        /// </summary>
        /// <param name="sudoku">The sudoku puzzle.</param>
        /// <param name="notes">The notes for sudoku puzzle.</param>
        /// <param name="cost">The cost of using the techniques.</param>
        /// <returns>Whether the technique has been applied or not.</returns>
        public abstract bool Apply(int[,] sudoku, bool[,] notes, out int cost);
    }
}