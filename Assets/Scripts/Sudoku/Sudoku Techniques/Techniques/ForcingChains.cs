using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/forcing-chains.
    /// </summary>
    public class ForcingChains : SudokuTechnique
    {
        protected override int FirstUseCost => 4200;
        protected override int SubsequentUseCost => 2100;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}