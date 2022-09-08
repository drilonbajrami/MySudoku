using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGridView : MonoBehaviour
{
    // Column & Row = 6 up to 24
    // Cross = 15 up to 35
    // Diagonal (1) = 1 up to 9
    // Diagonal (2) = 3 up to 17
    // Diagonal (3) = 6 up to 24

    /// <summary>
    /// Cell prefab.
    /// </summary>
    [SerializeField] private Cell _cellPrefab;

    /// <summary>
    /// Grid border prefab.
    /// </summary>
    [SerializeField] private Image _borderPrefab;

    /// <summary>
    /// Grid border thickness and length.
    /// </summary>
    private Vector2 _horizontalThick = new Vector2(906, 6);
    private Vector2 _horizontalSlim = new Vector2(906, 2);
    private Vector2 _verticalThick = new Vector2(6, 906);
    private Vector2 _verticalSlim = new Vector2(2, 906);

    /// <summary>
    /// Grid of cells.
    /// </summary>
    private Cell[,] _grid;

    [SerializeField] private SudokuResults _sudokuResults;

    private void Start()
    {
        SpawnCellGrid();
        FillGrid(_sudokuResults.GetCurrentSolution());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _sudokuResults.LoadNextFile();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _sudokuResults.LoadPreviousFile();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            FillGrid(_sudokuResults.GetNextSolution());
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            FillGrid(_sudokuResults.GetPreviousSolution());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int[,] data = SudokuData.GetPossiblePermutations(new int[] { 1, 2, 3 });

            for (int i = 0; i < data.GetLength(0); i++)
            {
                Debug.Log(i);
                Debug.Log($"Index: {data[i, 0]}, {data[i, 1]}, {data[i, 2]}");
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            int[,] data = SudokuData.GetPossiblePermutations(new int[] { 1, 2, 3, 4, 5, 6 });

            for (int i = 0; i < data.GetLength(0); i++)
            {
                Debug.Log(i);
                Debug.Log($"Index: {data[i, 0]}, {data[i, 1]}, {data[i, 2]}");
            }
        }

    }

    public void SpawnCellGrid()
    {
        ClearGrid();
        _grid = new Cell[9, 9];
        float cellLength = _cellPrefab.RectTransform.sizeDelta.x;
        float gridTopLeftPos = cellLength * 4.5f - cellLength * 0.5f;

        for(int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                Vector3 position = new Vector3(-gridTopLeftPos + x * cellLength, gridTopLeftPos - y * cellLength, 0);
                _grid[y, x] = Instantiate(_cellPrefab, transform);
                _grid[y, x].RectTransform.anchoredPosition = position;
            }
        }

        // Draw Borders
        for(int i = 0; i < 10; i++)
        {
            Image horizontalBorder = Instantiate(_borderPrefab, transform);
            Image verticalBorder = Instantiate(_borderPrefab, transform);
            horizontalBorder.rectTransform.anchoredPosition = new Vector2(0, -cellLength * 4.5f + i * cellLength);
            verticalBorder.rectTransform.anchoredPosition = new Vector2(cellLength * 4.5f - i * cellLength, 0);

            if (i % 3 == 0)
            {
                horizontalBorder.rectTransform.sizeDelta = _horizontalThick;
                verticalBorder.rectTransform.sizeDelta = _verticalThick;
            }
            else
            {
                horizontalBorder.rectTransform.sizeDelta = _horizontalSlim;
                verticalBorder.rectTransform.sizeDelta = _verticalSlim;
            }
        }
    }

    private void ClearGrid()
    {
        if (_grid == null) return;
        for (int y = 0; y < _grid.GetLength(0); y++)
            for (int x = 0; x < _grid.GetLength(1); x++)
            {
                if (_grid[y, x] == null)
                    return;

                Destroy(_grid[y, x].gameObject);
            }
    }

    public void FillGrid(List<int> digits)
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                _grid[y, x].SetDigit(digits[y * 9 + x]);
            }
        }

        CalculateSums();
    }

    private void CalculateSums()
    {
        for (int box = 0; box < 9; box++)
        {
            int x = box % 3 * 3;
            int y = box / 3 * 3;

            // Column and Row sums (6 to 24)
            for (int i = 0; i < 3; i++)
            {
                int colSum = _grid[x + i, y].Digit + _grid[x + i, y + 1].Digit + _grid[x + i, y + 2].Digit;
                _grid[x + i, y].SetColumnSum(colSum);

                int rowSum = _grid[x, y + i].Digit + _grid[x + 1, y + i].Digit + _grid[x + 2, y + i].Digit;
                _grid[x, y + i].SetRowSum(rowSum);
            }

            // Cross sum in one box (15 to 35)
            int crossSum = _grid[x + 1, y].Digit + _grid[x + 1, y + 1].Digit + _grid[x + 1, y + 2].Digit
                + _grid[x, y + 1].Digit + _grid[x + 2, y + 1].Digit;
            _grid[x + 1, y + 1].SetCrossSum(crossSum);

            // 5 L - (0,0) - (1,0) - (2,0) - (0,1) - (0,2)
            _grid[y, x].SetNWtoSESum(_grid[y,x].Digit + _grid[y + 1, x + 1].Digit + _grid[y + 2, x + 2].Digit);
            _grid[y, x + 1].SetNWtoSESum(_grid[y, x + 1].Digit + _grid[y + 1, x + 2].Digit);
            _grid[y, x + 2].SetNWtoSESum(_grid[y, x + 2].Digit);
            _grid[y + 1, x].SetNWtoSESum(_grid[y + 1, x].Digit + _grid[y + 2, x + 1].Digit);
            _grid[y + 2, x].SetNWtoSESum(_grid[y + 2, x].Digit);

            //// 5 R - (0,0) - (0,1) - (0,2) - (1,2) - (2,2)
            _grid[y, x].SetSWtoNESum(_grid[y, x].Digit);
            _grid[y + 1, x].SetSWtoNESum(_grid[y + 1, x].Digit + _grid[y, x + 1].Digit);
            _grid[y + 2, x].SetSWtoNESum(_grid[y + 2, x].Digit + _grid[y + 1, x + 1].Digit + _grid[y, x + 2].Digit);
            _grid[y + 2, x + 1].SetSWtoNESum(_grid[y + 2, x + 1].Digit + _grid[y + 1, x + 2].Digit);
            _grid[y + 2, x + 2].SetSWtoNESum(_grid[y + 2, x + 2].Digit);
        }
    }

    private void GetPermutations(int[] range)
    {
        int permutations = 0;
        for(int x = 0; x < range.Length; x++)
        {
            for(int y = 0; y < range.Length; y++)
            {
                for(int z = 0; z < range.Length; z++)
                {
                    if (x == y)
                        break;
                    else if (x == z)
                        continue;
                    else if (y == z)
                        continue;
                    else
                    {
                        permutations++;
                        Debug.Log($"{x + 1}, {y + 1}, {z + 1}");
                    }
                }
            }
        }

        Debug.Log($"{permutations} permutations.");
    }
}