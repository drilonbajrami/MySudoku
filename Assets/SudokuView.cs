using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SudokuView : MonoBehaviour, IPointerClickHandler
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

    // Useful is the index repetition of permutations in groups. Mabye use this within a rule if generating through human based
    // and recursive backtracking algorithm.

    /// <summary>
    /// Grid of cells.
    /// </summary>
    private Cell[,] _grid;

    /// <summary>
    /// Indexer for the grid of cells.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>The cell at the given coordinates.</returns>
    public Cell this[int x, int y] => _grid[y, x];

    private RectTransform _rectTransform;

    /// <summary>
    /// Cell prefab.
    /// </summary>
    [SerializeField] private Cell _cellPrefab;

    private Vector2Int _selectedCellIndex = new Vector2Int(-1, -1);

    /// <summary>
    /// Grid border prefab.
    /// </summary>
    [SerializeField] private Image _borderPrefab;
    [SerializeField] private float _cellSize = 100f;
    [Range(1f, 10f)][SerializeField] private float _borderThickness = 6f;

    [SerializeField] private SudokuResultsLibrary _sudokuResults;
    [SerializeField] private PermutationTableView _tableView;
    [SerializeField] private IndexRepetition _indexRepetition;

    bool stop = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

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
        float gridTopLeftPos = _cellSize * 4.5f - _cellSize * 0.5f;

        for(int y = 0; y < 9; y++) {
            for (int x = 0; x < 9; x++) {
                Vector3 position = new Vector3(-gridTopLeftPos + x * _cellSize, gridTopLeftPos - y * _cellSize, 0);
                _grid[y, x] = Instantiate(_cellPrefab, transform);
                _grid[y, x].RectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                _grid[y, x].RectTransform.anchoredPosition = position;
            }
        }

        GameObject borders = new GameObject();
        borders.transform.SetParent(transform);
        borders.transform.SetAsLastSibling();

        float _thickBorder = _borderThickness * _cellSize / 100f;
        float _slimBorder = _borderThickness / 4f * _cellSize / 100f;
        float _borderLength = _cellSize * 9 + _thickBorder;

        // Draw Borders
        for(int i = 0; i < 10; i++) {
            Image horizontalBorder = Instantiate(_borderPrefab, transform);
            Image verticalBorder = Instantiate(_borderPrefab, transform);
            horizontalBorder.rectTransform.anchoredPosition = new Vector2(0, -_cellSize * 4.5f + i * _cellSize);
            verticalBorder.rectTransform.anchoredPosition = new Vector2(_cellSize * 4.5f - i * _cellSize, 0);

            if (i % 3 == 0) {
                horizontalBorder.rectTransform.sizeDelta = new Vector2(_borderLength, _thickBorder);
                verticalBorder.rectTransform.sizeDelta = new Vector2(_thickBorder, _borderLength);
                horizontalBorder.rectTransform.SetParent(borders.transform);
                verticalBorder.rectTransform.SetParent(borders.transform);
            } else {
                horizontalBorder.rectTransform.sizeDelta = new Vector2(_borderLength, _slimBorder);
                verticalBorder.rectTransform.sizeDelta = new Vector2(_slimBorder, _borderLength);
                horizontalBorder.color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
                verticalBorder.color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
                horizontalBorder.rectTransform.SetParent(borders.transform);
                verticalBorder.rectTransform.SetParent(borders.transform);
                horizontalBorder.gameObject.transform.SetAsFirstSibling();
                verticalBorder.gameObject.transform.SetAsFirstSibling();
            }
        }
;
        _rectTransform.sizeDelta = new Vector2(_borderLength, _borderLength);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 sudokuPosition = new Vector2(eventData.pressPosition.x - Screen.width / 2f - _rectTransform.localPosition.x, 
                                             eventData.pressPosition.y - Screen.height / 2f - _rectTransform.localPosition.y);

        Vector2 sudokuLocalPosition = new Vector2(sudokuPosition.x + _cellSize * 4.5f, -sudokuPosition.y + _cellSize * 4.5f);
        int x = (int)(sudokuLocalPosition.x / _cellSize);
        int y = (int)(sudokuLocalPosition.y / _cellSize);

        if (0 <= x && x <= 8 && 0 <= y && y <= 8)
        {
            if(_selectedCellIndex.x == -1 && _selectedCellIndex.y == -1) 
            { 
                _selectedCellIndex = new Vector2Int(x, y);
                this[x, y].Select();
                HighlightSelectedCellsNeighbours(x, y, true);
            } 
            else if(_selectedCellIndex.x == x && _selectedCellIndex.y == y) 
            {
                this[x, y].Deselect();
                HighlightSelectedCellsNeighbours(x, y, false);
                _selectedCellIndex = new Vector2Int(-1, -1);
            } 
            else 
            {
                Debug.Log(_selectedCellIndex);
                this[_selectedCellIndex.x, _selectedCellIndex.y].Deselect();
                HighlightSelectedCellsNeighbours(_selectedCellIndex.x, _selectedCellIndex.y, false);
                _selectedCellIndex = new Vector2Int(x, y);
                this[x, y].Select();
                HighlightSelectedCellsNeighbours(x, y, true);
            }
        }
    }

    private void HighlightSelectedCellsNeighbours(int selectedX, int selectedY, bool highlight)
    {
        int box = Mathf.FloorToInt(selectedX / 3f) + 3 * Mathf.FloorToInt(selectedY / 3f);
        int x = box % 3 * 3;
        int y = box / 3 * 3;

        for (int j = 0; j < 9; j++)
            if (j != selectedY)
                this[selectedX, j].Highlight(highlight);

        for (int i = 0; i < 9; i++)
            if (i != selectedX)
                this[i, selectedY].Highlight(highlight);

        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (x + i != selectedX && y + j != selectedY)
                    this[x + i, y + j].Highlight(highlight);
            }
        }

        for (int j = 0; j < 9; j++)
            for (int i = 0; i < 9; i++)
            {
                if (this[selectedX, selectedY].Digit == this[i, j].Digit && selectedX != i && selectedY != j)
                    if (highlight) this[i, j].Select();
                    else this[i, j].Deselect();
            }
    }
}