using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudoku.org.uk/SolvingTechniques/NakedQuads.asp.
    /// </summary>
    public class NakedQuads : SudokuTechnique
    {
        protected override int FirstUseCost => 5000;
        protected override int SubsequentUseCost => 4000;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            return false;
        }
    }
}