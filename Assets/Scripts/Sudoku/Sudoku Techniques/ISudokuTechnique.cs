using System.Collections;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Sudoku Technique interface.
    /// </summary>
    public interface ISudokuTechnique
    {
        /// <summary>
        /// Keep track of the ties this technique has been used.
        /// </summary>
        public int TimesUsed { get; set; }

        /// <summary>
        /// The cost of this technique on its first use.
        /// </summary>
        public int FirstUseCost { get; }

        /// <summary>
        /// The cost of this technique on its subsequent uses.
        /// </summary>
        public int SubsequentUseCost { get; }

        /// <summary>
        /// Should this technique log its process on the console.
        /// </summary>
        public bool LogConsole { get; set; }

        /// <summary>
        /// Checks if this technique can be applied.
        /// </summary>
        /// <param name="sudoku">The sudoku puzzle.</param>
        /// <param name="notes">The notes for sudoku puzzle.</param>
        /// <returns>Whether the technique has been applied or not.</returns>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost);
    }
}