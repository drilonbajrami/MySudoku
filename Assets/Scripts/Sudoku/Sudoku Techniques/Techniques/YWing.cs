using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://sudoku.com/sudoku-rules/y-wing/.
    /// </summary>
    public class YWing : SudokuTechnique
    {
        protected override int FirstUseCost => 3000;
        protected override int SubsequentUseCost => 1800;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}