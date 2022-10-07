using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sudokutils
{
    /// <summary>
    /// Calculates different sums of the solution numbers within the grid.
    /// </summary>
    //public static void CalculateSums(Cell[,] _grid)
    //{
    //    for (int box = 0; box < 9; box++) {
    //        int row = box / 3 * 3;
    //        int col = box % 3 * 3;

    //        for (int i = 0; i < 3; i++) {
    //            int colSum = _grid[row, col + i].Number +
    //                         _grid[row + 1, col + i].Number +
    //                         _grid[row + 2, col + i].Number;
    //            _grid[row, col + i].Sums.SetColumnSum(colSum);

    //            int rowSum = _grid[row + i, col].Number +
    //                         _grid[row + i, col + 1].Number +
    //                         _grid[row + i, col + 2].Number;
    //            _grid[row + i, col].Sums.SetRowSum(rowSum);
    //        }

    //        // Cross sum in one box (15 to 35)
    //        int crossSum = _grid[row, col + 1].Number +
    //                       _grid[row + 1, col + 1].Number +
    //                       _grid[row + 2, col + 1].Number +
    //                       _grid[row + 1, col].Number +
    //                       _grid[row + 1, col + 2].Number;
    //        _grid[row + 1, col + 1].Sums.SetCrossSum(crossSum);

    //        // Diagonal sums from top left to bottom right
    //        _grid[row, col].Sums.SetTLBRSum(_grid[row, col].Number +
    //                                        _grid[row + 1, col + 1].Number +
    //                                        _grid[row + 2, col + 2].Number);
    //        _grid[row, col + 1].Sums.SetTLBRSum(_grid[row, col + 1].Number +
    //                                            _grid[row + 1, col + 2].Number);
    //        _grid[row, col + 2].Sums.SetTLBRSum(_grid[row, col + 2].Number);
    //        _grid[row + 1, col].Sums.SetTLBRSum(_grid[row + 1, col].Number +
    //                                            _grid[row + 2, col + 1].Number);
    //        _grid[row + 2, col].Sums.SetTLBRSum(_grid[row + 2, col].Number);

    //        // Diagonal sums from bottom left to top right
    //        _grid[row, col].Sums.SetBLTRSum(_grid[row, col].Number);
    //        _grid[row + 1, col].Sums.SetBLTRSum(_grid[row + 1, col].Number +
    //                                            _grid[row, col + 1].Number);
    //        _grid[row + 2, col].Sums.SetBLTRSum(_grid[row + 2, col].Number +
    //                                            _grid[row + 1, col + 1].Number +
    //                                            _grid[row, col + 2].Number);
    //        _grid[row + 2, col + 1].Sums.SetBLTRSum(_grid[row + 2, col + 1].Number +
    //                                                _grid[row + 1, col + 2].Number);
    //        _grid[row + 2, col + 2].Sums.SetBLTRSum(_grid[row + 2, col + 2].Number);
    //    }
    //}
}