using System.Collections;
using System.Collections.Generic;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/forcing-chains.
    /// </summary>
    public class ForcingChains : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 4200;

        /// <inheritdoc/>
        public int SubsequentUseCost => 2100;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            return false;
        }
    }
}