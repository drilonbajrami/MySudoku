using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/x-wings.
    /// </summary>
    public class XWing : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 2800;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1600;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            return false;
        }
    }
}