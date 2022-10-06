using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SudokuTesting;

namespace MySudoku
{
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

        // A combination of 3 numbers {x, y, z} can appear only 4 times max in a sudoku puzzle

        /// <summary>
        /// Grid of cells.
        /// </summary>
        private readonly Cell[,] _grid = new Cell[9, 9];

        private RectTransform _rectTransform;

        /// <summary>
        /// Cell prefab.
        /// </summary>
        [SerializeField] private Cell _cellPrefab;

        private (int row, int col) _selectedCell = (-1, -1);

        /// <summary>
        /// Grid border prefab.
        /// </summary>
        [SerializeField] private Image _borderPrefab;
        [SerializeField] private float _cellSize = 100f;
        [Range(1f, 10f)][SerializeField] private float _borderThickness = 6f;

        [SerializeField] private SudokuResultsLibrary _sudokuResults;
        [SerializeField] private PermutationTableView _tableView;

        bool stop = false;

        public bool _showPermutationGroups;

        public SudokuGenerator _generator;

        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        /// <summary>
        /// Draws the sudoku grid and fills it with the current solution loaded in the sudoku results library.
        /// </summary>
        private void Start()
        {
            DrawSudoku();
            FillGrid(_sudokuResults.GetCurrentSolution());
        }

        Sudoku sud;

        private void Update()
        {
            CycleSudokuSolutions();

            if (Input.GetKeyDown(KeyCode.Q)) FillGridSimple();
            if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(CheckPermutationsCoroutine());
            else if (Input.GetKeyDown(KeyCode.P)) StopAllCoroutines();
            if (Input.GetKeyDown(KeyCode.O)) foreach (Cell cell in _grid) cell.ToggleSums();

            if (Input.GetKeyDown(KeyCode.E)) sud = _generator.Generate();
            if (Input.GetKeyDown(KeyCode.T)) FillGrid(sud.GetSolution());
            else if (Input.GetKeyDown(KeyCode.R)) FillGrid(sud.GetPuzzle());
        }

        #region Base Sudoku Grid Methods
        /// <summary>
        /// Draws the sudoku grid.
        /// </summary>
        public void DrawSudoku()
        {
            ClearGrid();
            SpawnCells();
            SpawnBorders();
        }

        private void SpawnCells()
        {
            RectTransform cells = new GameObject().AddComponent<RectTransform>();
            cells.SetParent(transform);
            cells.anchoredPosition = Vector2.zero;
            float gridTopLeftPos = _cellSize * 4.5f - _cellSize * 0.5f;

            for (int row = 0; row < 9; row++) {
                for (int col = 0; col < 9; col++) {
                    Vector3 position = new Vector3(-gridTopLeftPos + col * _cellSize, gridTopLeftPos - row * _cellSize, 0);
                    _grid[row, col] = Instantiate(_cellPrefab, cells);
                    _grid[row, col].RectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                    _grid[row, col].RectTransform.anchoredPosition = position;
                }
            }
        }

        private void SpawnBorders()
        {
            RectTransform borders = new GameObject().AddComponent<RectTransform>();
            borders.SetParent(transform);
            borders.anchoredPosition = Vector2.zero;

            float thickBorder = _borderThickness * _cellSize / 100f;
            float slimBorder = _borderThickness / 4f * _cellSize / 100f;
            float borderLength = _cellSize * 9 + thickBorder;

            // Draw Borders
            for (int i = 0; i < 10; i++) {
                Image horizontalBorder = Instantiate(_borderPrefab, transform);
                Image verticalBorder = Instantiate(_borderPrefab, transform);
                horizontalBorder.rectTransform.anchoredPosition = new Vector2(0, -_cellSize * 4.5f + i * _cellSize);
                verticalBorder.rectTransform.anchoredPosition = new Vector2(_cellSize * 4.5f - i * _cellSize, 0);

                if (i % 3 == 0) {
                    horizontalBorder.rectTransform.sizeDelta = new Vector2(borderLength, thickBorder);
                    verticalBorder.rectTransform.sizeDelta = new Vector2(thickBorder, borderLength);
                    horizontalBorder.rectTransform.SetParent(borders.transform);
                    verticalBorder.rectTransform.SetParent(borders.transform);
                }
                else {
                    horizontalBorder.rectTransform.sizeDelta = new Vector2(borderLength, slimBorder);
                    verticalBorder.rectTransform.sizeDelta = new Vector2(slimBorder, borderLength);
                    horizontalBorder.color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
                    verticalBorder.color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
                    horizontalBorder.rectTransform.SetParent(borders.transform);
                    verticalBorder.rectTransform.SetParent(borders.transform);
                    horizontalBorder.gameObject.transform.SetAsFirstSibling();
                    verticalBorder.gameObject.transform.SetAsFirstSibling();
                }
            }
        ;
            _rectTransform.sizeDelta = new Vector2(borderLength, borderLength);
        }

        /// <summary>
        /// Fills the sudoku grid with the given numbers/solution.
        /// </summary>
        /// <param name="digits">The sudoku solution.</param>
        public void FillGrid(List<int> digits)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _grid[row, col].SetNum(digits[row * 9 + col]);

            //CalculateSums();
            //CheckPermutationsAndIndexes();
        }

        /// <summary>
        /// Clears the grid of all cells game objects.
        /// </summary>
        private void ClearGrid()
        {
            for (int row = 0; row < _grid.GetLength(0); row++)
                for (int col = 0; col < _grid.GetLength(1); col++) {
                    if (_grid[row, col] == null) return;
                    Destroy(_grid[row, col].gameObject);
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
        public void FillGridSimple() =>
            // Simplest sudoku to create with 2 permutation repeated 3 times per group
            FillGrid(new List<int> { 1,2,3,4,5,6,7,8,9,4,5,6,7,8,9,1,2,3,7,8,9,1,2,3,4,5,6,
                                 2,3,4,5,6,7,8,9,1,5,6,7,8,9,1,2,3,4,8,9,1,2,3,4,5,6,7,
                                 3,4,5,6,7,8,9,1,2,6,7,8,9,1,2,3,4,5,9,1,2,3,4,5,6,7,8 });

        /// <summary>
        /// Calculates different sums of the solution numbers within the grid.
        /// </summary>
        private void CalculateSums()
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

        /// <summary>
        /// Checks all the permutations.
        /// </summary>
        private void CheckPermutations()
        {
            int[] permutation = new int[3];
            for (int box = 0; box < 9; box++) {
                int row = box / 3 * 3;
                int col = box % 3 * 3;

                for (int i = 0; i < 3; i++) {
                    // Horizontal direction
                    permutation[0] = _grid[row + i, col].Number;
                    permutation[1] = _grid[row + i, col + 1].Number;
                    permutation[2] = _grid[row + i, col + 2].Number;

                    if (permutation[0] != 0 && permutation[1] != 0 && permutation[2] != 0)
                        stop = _tableView.CheckPermutation(permutation, true, box + 1, i == 0);

                    // Vertical direction
                    permutation[0] = _grid[row, col + i].Number;
                    permutation[1] = _grid[row + 1, col + i].Number;
                    permutation[2] = _grid[row + 2, col + i].Number;

                    if (permutation[0] != 0 && permutation[1] != 0 && permutation[2] != 0)
                        stop = _tableView.CheckPermutation(permutation, false, box + 1, i == 0);
                }
            }
        }

        /// <summary>
        /// Checks the permutation and indexes.
        /// </summary>
        private void CheckPermutationsAndIndexes()
        {
            _tableView.UncheckPermutations();
            CheckPermutations();
        }

        /// <summary>
        /// Checks the permutation and indexes as a coroutine.
        /// </summary>
        private IEnumerator CheckPermutationsCoroutine()
        {
            for (int i = 0; i < 100000; i++) {
                FillGrid(_sudokuResults.GetNextSolution());
                if (stop) StopAllCoroutines();
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Handles clicks on the sudoku grid.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            (int row, int col) index = GetClickedCellIndex(eventData.pressPosition);
            HandleClickedCell(index.row, index.col);
        }

        /// <summary>
        /// Gets the clicked cell row and column index.
        /// </summary>
        /// <param name="pressPosition">Mouse press position.</param>
        /// <returns>Row and column index.</returns>
        private (int, int) GetClickedCellIndex(Vector2 pressPosition)
        {
            Vector2 sudokuPos = new(pressPosition.x - Screen.width / 2f - _rectTransform.localPosition.x,
                                          pressPosition.y - Screen.height / 2f - _rectTransform.localPosition.y);
            Vector2 sudokuLocalPos = new Vector2(sudokuPos.x + _cellSize * 4.5f, -sudokuPos.y + _cellSize * 4.5f);
            int row = (int)(sudokuLocalPos.y / _cellSize);
            int col = (int)(sudokuLocalPos.x / _cellSize);
            return (row, col);
        }

        /// <summary>
        /// Handles the clicked cell at given row and column index.
        /// </summary>
        /// <param name="row">Row index of the clicked cell.</param>
        /// <param name="col">Column index of the clicked cell.</param>
        private void HandleClickedCell(int row, int col)
        {
            if (0 <= row && row <= 8 && 0 <= col && col <= 8) {
                if (_selectedCell.row == -1 && _selectedCell.col == -1) {
                    _selectedCell = (row, col);
                    _grid[row, col].Select(HighlightNeighbors);
                }
                else if (row == _selectedCell.row && col == _selectedCell.col) {
                    _grid[row, col].Deselect(HighlightNeighbors);
                    _selectedCell = (-1, -1);
                }
                else {
                    _grid[_selectedCell.row, _selectedCell.col].Deselect(HighlightNeighbors);
                    _selectedCell = (row, col);
                    _grid[row, col].Select(HighlightNeighbors);
                }
            }
        }

        /// <summary>
        /// Highlights or un-highlights neighbors of the currently selected cell based on the given condition.
        /// </summary>
        /// <param name="on">Whether to highlight neighbor cells or not.</param>
        private void HighlightNeighbors(bool on)
        {
            int box = Mathf.FloorToInt(_selectedCell.row / 3f) * 3 + Mathf.FloorToInt(_selectedCell.col / 3f);
            int boxRow = box / 3 * 3;
            int boxCol = box % 3 * 3;

            for (int i = 0; i < 9; i++) {
                if (i != _selectedCell.row) _grid[i, _selectedCell.col].Focus(on);
                if (i != _selectedCell.col) _grid[_selectedCell.row, i].Focus(on);

                int r = i / 3;
                int c = i % 3;
                if (boxRow + r != _selectedCell.row && boxCol + c != _selectedCell.col)
                    _grid[boxRow + r, boxCol + c].Focus(on);

                for (int j = 0; j < 9; j++)
                    if (_grid[_selectedCell.row, _selectedCell.col].Number == _grid[i, j].Number
                     && _selectedCell.row != i
                     && _selectedCell.col != j
                     && _grid[_selectedCell.row, _selectedCell.col].Number != 0)
                        if (on) _grid[i, j].Select(null);
                        else _grid[i, j].Deselect(null);
            }
        }

        public void HighlightCells(int[] permutation, int box, bool highlight)
        {
            int boxIndex = box - 1;
            int col = boxIndex % 3 * 3;
            int row = boxIndex / 3 * 3;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    for (int p = 0; p < 3; p++)
                        if (_grid[row + r, col + c].Number == permutation[p])
                            _grid[row + r, col + c].Focus(highlight, Color.red);
        }
    }
}