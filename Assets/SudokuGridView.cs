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

    // 6 permutations per group (all permutations starting with number x are considered a group in itself)
    // out of 6 permutations per group, 3 permutations are in horizontal direction, and 3 are in vertical direction
    // A same permutation in a group can be present up to 3 times max. with (1-2) or (2-1) - horizontal to vertical ratio directions.
    // The sums of horizontal and diagonal permutations (triplets) per box are little to no use since there are many combinations with it
    // that there might not be a way to rely on them for generating a sudoku puzzle 
    
    // Useful is the index repetition of permutations in groups. Mabye use this within a rule if generating through human based and recursive backtracking algorithm.

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

    public Cell this[int x, int y] => _grid[y, x];

    [SerializeField] private SudokuResultsLibrary _sudokuResults;
    [SerializeField] private PermutationTableView _tableView;
    [SerializeField] private IndexRepetition _indexRepetition;

    bool stop = false;

    /// <summary>
    /// Draws the sudoku grid and fills it with the current solution loaded in the sudoku results library.
    /// </summary>
    private void Start()
    {
        DrawSudoku();
        FillGrid(_sudokuResults.GetCurrentSolution());
    }

    private void Update()
    {
        CycleSudokuSolutions();

        if (Input.GetKeyDown(KeyCode.Q)) FillGridSimple();
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(CheckPermutationsCoroutine());
        else if (Input.GetKeyDown(KeyCode.P)) StopAllCoroutines();  
        if(Input.GetKeyDown(KeyCode.O)) foreach (Cell cell in _grid) cell.ToggleSums();  
    }

    #region Base Sudoku Grid Methods
    /// <summary>
    /// Draws the sudoku grid.
    /// </summary>
    public void DrawSudoku()
    {
        ClearGrid();
        _grid = new Cell[9, 9];
        float cellLength = _cellPrefab.RectTransform.sizeDelta.x;
        float gridTopLeftPos = cellLength * 4.5f - cellLength * 0.5f;

        for(int y = 0; y < 9; y++) {
            for (int x = 0; x < 9; x++) {
                Vector3 position = new Vector3(-gridTopLeftPos + x * cellLength, gridTopLeftPos - y * cellLength, 0);
                _grid[y, x] = Instantiate(_cellPrefab, transform);
                _grid[y, x].RectTransform.anchoredPosition = position;
            }
        }

        // Draw Borders
        for(int i = 0; i < 10; i++) {
            Image horizontalBorder = Instantiate(_borderPrefab, transform);
            Image verticalBorder = Instantiate(_borderPrefab, transform);
            horizontalBorder.rectTransform.anchoredPosition = new Vector2(0, -cellLength * 4.5f + i * cellLength);
            verticalBorder.rectTransform.anchoredPosition = new Vector2(cellLength * 4.5f - i * cellLength, 0);

            if (i % 3 == 0) {
                horizontalBorder.rectTransform.sizeDelta = _horizontalThick;
                verticalBorder.rectTransform.sizeDelta = _verticalThick;
            } else {
                horizontalBorder.rectTransform.sizeDelta = _horizontalSlim;
                verticalBorder.rectTransform.sizeDelta = _verticalSlim;
            }
        }
    }

    /// <summary>
    /// Fills the sudoku grid with the given numbers/solution.
    /// </summary>
    /// <param name="digits">The sudoku solution.</param>
    public void FillGrid(List<int> digits)
    {
        for (int y = 0; y < 9; y++)
            for (int x = 0; x < 9; x++)
                this[x, y].SetDigit(digits[y * 9 + x]);

        CalculateSums();
        CheckPermutationsAndIndexes();
    }

    /// <summary>
    /// Fills the sudoku grid with the simplest number order
    /// </summary>
    public void FillGridSimple()
    {
        // Simplest sudoku to create with 2 permutation repeated 3 times per group
        FillGrid(new List<int> { 1,2,3,4,5,6,7,8,9,4,5,6,7,8,9,1,2,3,7,8,9,1,2,3,4,5,6,
                                 2,3,4,5,6,7,8,9,1,5,6,7,8,9,1,2,3,4,8,9,1,2,3,4,5,6,7,
                                 3,4,5,6,7,8,9,1,2,6,7,8,9,1,2,3,4,5,9,1,2,3,4,5,6,7,8 });
    }

    /// <summary>
    /// Clears the grid of all cells game objects.
    /// </summary>
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

    /// <summary>
    /// Cycles through the sudoku solution files and singe solution through key presses.
    /// </summary>
    private void CycleSudokuSolutions()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            _sudokuResults.LoadNextFile();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _sudokuResults.LoadPreviousFile();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            FillGrid(_sudokuResults.GetNextSolution());
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            FillGrid(_sudokuResults.GetPreviousSolution());
    }
    #endregion

    /// <summary>
    /// Calculates different sums of the solution numbers within the grid.
    /// </summary>
    private void CalculateSums()
    {
        for (int box = 0; box < 9; box++)
        {
            int x = box % 3 * 3;
            int y = box / 3 * 3;

            for (int i = 0; i < 3; i++)
            {
                int colSum = _grid[x + i, y].Digit + _grid[x + i, y + 1].Digit + _grid[x + i, y + 2].Digit;
                _grid[x + i, y].Sums.SetColumnSum(colSum);

                int rowSum = _grid[x, y + i].Digit + _grid[x + 1, y + i].Digit + _grid[x + 2, y + i].Digit;
                _grid[x, y + i].Sums.SetRowSum(rowSum);
            }

            // Cross sum in one box (15 to 35)
            int crossSum = _grid[x + 1, y].Digit + _grid[x + 1, y + 1].Digit + _grid[x + 1, y + 2].Digit
                + _grid[x, y + 1].Digit + _grid[x + 2, y + 1].Digit;
            _grid[x + 1, y + 1].Sums.SetCrossSum(crossSum);

            // Diagonal sums from top left to bottom right
            _grid[y, x].Sums.SetTLBRSum(_grid[y, x].Digit + _grid[y + 1, x + 1].Digit + _grid[y + 2, x + 2].Digit);
            _grid[y, x + 1].Sums.SetTLBRSum(_grid[y, x + 1].Digit + _grid[y + 1, x + 2].Digit);
            _grid[y, x + 2].Sums.SetTLBRSum(_grid[y, x + 2].Digit);
            _grid[y + 1, x].Sums.SetTLBRSum(_grid[y + 1, x].Digit + _grid[y + 2, x + 1].Digit);
            _grid[y + 2, x].Sums.SetTLBRSum(_grid[y + 2, x].Digit);

            // Diagonal sums from bottom left to top right
            _grid[y, x].Sums.SetBLTRSum(_grid[y, x].Digit);
            _grid[y + 1, x].Sums.SetBLTRSum(_grid[y + 1, x].Digit + _grid[y, x + 1].Digit);
            _grid[y + 2, x].Sums.SetBLTRSum(_grid[y + 2, x].Digit + _grid[y + 1, x + 1].Digit + _grid[y, x + 2].Digit);
            _grid[y + 2, x + 1].Sums.SetBLTRSum(_grid[y + 2, x + 1].Digit + _grid[y + 1, x + 2].Digit);
            _grid[y + 2, x + 2].Sums.SetBLTRSum(_grid[y + 2, x + 2].Digit);
        }
    }

    /// <summary>
    /// Checks all the permutations.
    /// </summary>
    private void CheckPermutations()
    {
        int[] permutation = new int[3];

        for (int box = 0; box < 9; box++)
        {
            int x = box % 3 * 3;
            int y = box / 3 * 3;

            for (int j = 0; j < 3; j++)
            {
                // Horizontal direction
                permutation[0] = this[x, y + j].Digit;
                permutation[1] = this[x + 1, y + j].Digit;
                permutation[2] = this[x + 2, y + j].Digit;
                stop = _tableView.CheckPermutation(permutation, true, box + 1);

                // Vertical direction
                permutation[0] = this[x + j, y].Digit;
                permutation[1] = this[x + j, y + 1].Digit;
                permutation[2] = this[x + j, y + 2].Digit;
                stop = _tableView.CheckPermutation(permutation, false, box + 1);

                if (_indexRepetition != null)
                {
                    _indexRepetition.RegisterIndex(SudokuData.FindPermutationIndex(permutation));
                    _indexRepetition.RegisterIndex(SudokuData.FindPermutationIndex(permutation));
                }
            }
        }
    }

    /// <summary>
    /// Checks the permutation and indexes.
    /// </summary>
    private void CheckPermutationsAndIndexes()
    {
        if (_indexRepetition != null && _indexRepetition.gameObject.activeSelf)
            _indexRepetition.ClearRegister();

        _tableView.UncheckPermutations();
        CheckPermutations();

        if (_indexRepetition != null && _indexRepetition.gameObject.activeSelf)
            _indexRepetition.UpdateRegisterTable();
    }

    /// <summary>
    /// Checks the permutation and indexes as a coroutine.
    /// </summary>
    private IEnumerator CheckPermutationsCoroutine()
    {
        for (int i = 0; i < 100000; i++)
        {
            FillGrid(_sudokuResults.GetNextSolution());
            if (stop) StopAllCoroutines();
            yield return new WaitForEndOfFrame();
        }
    }
}