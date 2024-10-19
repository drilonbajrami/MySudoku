using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/x-wings.
    /// </summary>
    public class XWing : SudokuTechnique
    {
        protected override int FirstUseCost => 2800;
        protected override int SubsequentUseCost => 1600;

        public override bool Apply(int[,] sudoku, bool[,,] notes, out int cost)
        {
            // store information per each candidate - row it is on and column as well
            // for row - if candidate is present on two cells only on two rows on the same cols then remove everything on those cols.
            // for col - if candidate is present on two cells only on two cols on the same rows then remove everything on those rows.
            // return true only if that candidate has been removed from other cells.

            //Repetition[] candidates = new Repetition[9] {
            //    new Repetition(0), new Repetition(1), new Repetition(2),
            //    new Repetition(3), new Repetition(4), new Repetition(5),
            //    new Repetition(6), new Repetition(7), new Repetition(8)
            //};

            //for(int row = 0; row < 9; row++)
            //    for(int col = 0; col < 9; col++) {

            //    }

            //for (int n = 0; n < 9; n++) // Candidate's index as [n].
            //    for (int row = 0; row < 9; row++)
            //        for (int col = 0; col < 9; col++) {
            //            if (notes[row * 9 + col, n] && sudoku[row, col] == 0) {
            //                candidates[n].Row.Add(col);
            //                candidates[n].Col.Add(row);
            //            }
            //        }

            //for(int n = 0; n < 9; n++) {

            //    // Row ->
            //    if (candidates[n].Index )
            //}

            cost = 0;
            return false;
        }
    }
}