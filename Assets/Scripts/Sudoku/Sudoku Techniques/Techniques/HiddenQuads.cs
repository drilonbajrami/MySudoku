using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudoku.org.uk/SolvingTechniques/HiddenTriples.asp.
    /// </summary>
    public class HiddenQuads : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 7000;

        /// <inheritdoc/>
        public int SubsequentUseCost => 5000;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            return false;
        }
    }
}