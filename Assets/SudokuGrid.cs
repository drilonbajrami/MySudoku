using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    // Empty grid ( 0 = empty cell )
    private int[,] _grid = new int[9, 9]
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

    public void PlaceDigit(int i, int j, int digit)
    {
        _grid[i, j] = digit;
    }

    //public int RemoveDigit(int i, int j)
    //{
    //    int digit = _grid[i, j];
    //    _grid[i, j] = 0;

    //}
}
