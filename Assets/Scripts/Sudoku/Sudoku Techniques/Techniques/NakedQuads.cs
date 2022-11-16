using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudoku.org.uk/SolvingTechniques/NakedQuads.asp.
    /// </summary>
    public class NakedQuads : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 5000;

        /// <inheritdoc/>
        public int SubsequentUseCost => 4000;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}