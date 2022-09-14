using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    // Empty grid ( 0 = empty cell )
    private readonly int[,] _grid = new int[9, 9]
    {
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    /// <summary>
    /// Places the given digit on the given coordinates of the grid.
    /// </summary>
    /// <param name="i">The first (x) coordinate.</param>
    /// <param name="j">The second (y) coordinate.</param>
    /// <param name="digit">The digit to place.</param>
    public void PlaceDigit(int i, int j, int digit)
    {
        _grid[i, j] = digit;
    }
}