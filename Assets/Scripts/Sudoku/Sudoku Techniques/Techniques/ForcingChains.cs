using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/forcing-chains.
    /// </summary>
    public class ForcingChains : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; private set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 4200;

        /// <inheritdoc/>
        public int SubsequentUseCost => 2100;

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