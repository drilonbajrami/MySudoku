using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudoku.org.uk/SolvingTechniques/HiddenTriples.asp.
    /// </summary>
    public class HiddenQuads : SudokuTechnique
    {
        protected override int FirstUseCost => 7000;
        protected override int SubsequentUseCost => 5000;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}