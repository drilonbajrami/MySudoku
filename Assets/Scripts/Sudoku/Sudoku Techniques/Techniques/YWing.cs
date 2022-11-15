using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://sudoku.com/sudoku-rules/y-wing/.
    /// </summary>
    public class YWing : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 3000;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1800;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            return false;
        }
    }
}