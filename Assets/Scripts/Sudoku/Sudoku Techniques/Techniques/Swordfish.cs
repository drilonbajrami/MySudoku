using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/swordfish.
    /// </summary>
    public class Swordfish : SudokuTechnique
    {
        protected override int FirstUseCost => 8000;
        protected override int SubsequentUseCost => 6000;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}