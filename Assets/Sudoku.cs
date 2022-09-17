using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sudoku : MonoBehaviour
{
    public int[,] Grid => _grid;
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
    /// Sets the given digit on the given coordinates of the grid.
    /// </summary>
    /// <param name="i">The first (x) coordinate.</param>
    /// <param name="j">The second (y) coordinate.</param>
    /// <param name="digit">The digit to place.</param>
    public void SetDigit(int i, int j, int digit)
    {
        _grid[i, j] = digit;
    }
}