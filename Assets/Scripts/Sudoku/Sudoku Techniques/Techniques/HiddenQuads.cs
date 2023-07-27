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
        public int TimesUsed { get; private set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 7000;

        /// <inheritdoc/>
        public int SubsequentUseCost => 5000;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public void ResetUseCount() => TimesUsed = 0;

        /// <inheritdoc/>
        public bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}