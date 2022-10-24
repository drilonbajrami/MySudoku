using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sudokutils
{
    /// <summary>
    /// Calculates different sums of the solution numbers within the grid.
    /// </summary>
    public static void CalculateSums(Cell[,] _grid)
    {
        for (int box = 0; box < 9; box++) {
            int row = box / 3 * 3;
            int col = box % 3 * 3;

            for (int i = 0; i < 3; i++) {
                int colSum = _grid[row, col + i].Number +
                             _grid[row + 1, col + i].Number +
                             _grid[row + 2, col + i].Number;
                _grid[row, col + i].Sums.SetColumnSum(colSum);

                int rowSum = _grid[row + i, col].Number +
                             _grid[row + i, col + 1].Number +
                             _grid[row + i, col + 2].Number;
                _grid[row + i, col].Sums.SetRowSum(rowSum);
            }

            // Cross sum in one box (15 to 35)
            int crossSum = _grid[row, col + 1].Number +
                           _grid[row + 1, col + 1].Number +
                           _grid[row + 2, col + 1].Number +
                           _grid[row + 1, col].Number +
                           _grid[row + 1, col + 2].Number;
            _grid[row + 1, col + 1].Sums.SetCrossSum(crossSum);

            // Diagonal sums from top left to bottom right
            _grid[row, col].Sums.SetTLBRSum(_grid[row, col].Number +
                                            _grid[row + 1, col + 1].Number +
                                            _grid[row + 2, col + 2].Number);
            _grid[row, col + 1].Sums.SetTLBRSum(_grid[row, col + 1].Number +
                                                _grid[row + 1, col + 2].Number);
            _grid[row, col + 2].Sums.SetTLBRSum(_grid[row, col + 2].Number);
            _grid[row + 1, col].Sums.SetTLBRSum(_grid[row + 1, col].Number +
                                                _grid[row + 2, col + 1].Number);
            _grid[row + 2, col].Sums.SetTLBRSum(_grid[row + 2, col].Number);

            // Diagonal sums from bottom left to top right
            _grid[row, col].Sums.SetBLTRSum(_grid[row, col].Number);
            _grid[row + 1, col].Sums.SetBLTRSum(_grid[row + 1, col].Number +
                                                _grid[row, col + 1].Number);
            _grid[row + 2, col].Sums.SetBLTRSum(_grid[row + 2, col].Number +
                                                _grid[row + 1, col + 1].Number +
                                                _grid[row, col + 2].Number);
            _grid[row + 2, col + 1].Sums.SetBLTRSum(_grid[row + 2, col + 1].Number +
                                                    _grid[row + 1, col + 2].Number);
            _grid[row + 2, col + 2].Sums.SetBLTRSum(_grid[row + 2, col + 2].Number);
        }
    }

    #region Findings
    // Column & Row = 6 up to 24
    // Cross = 15 up to 35
    // Diagonal (1) = 1 up to 9
    // Diagonal (2) = 3 up to 17
    // Diagonal (3) = 6 up to 24

    // 6 permutations per group (all permutations starting with number x are considered a group in itself)
    // out of 6 permutations per group, 3 permutations are in horizontal direction, and 3 are in vertical direction
    // A same permutation in a group can be present up to 3 times max. with (1-2) or (2-1) - horizontal to vertical ratio directions.
    // The sums of horizontal and diagonal permutations (triplets) per box are little to no use since there are many combinations with it
    // that there might not be a way to rely on them for generating a sudoku puzzle 

    // Useful is the index repetition of permutations in groups. Mabye use this within a rule if generating through human based
    // and recursive backtracking algorithm.

    // A combination of 3 numbers {x, y, z} can appear only 4 times max in a sudoku puzzle
    #endregion
}