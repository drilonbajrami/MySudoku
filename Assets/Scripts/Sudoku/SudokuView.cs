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
        /// The sudoku to show/draw.
        /// </summary>
        private Sudoku _sudoku;

        /// <summary>
        /// Grid of cells.
        /// </summary>
        private readonly Cell[,] _grid = new Cell[9, 9];

        /// <summary>
        /// The rect transform of the sudoku view.
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// Cell prefab.
        /// </summary>
        [SerializeField] private Cell _cellPrefab;
        /// <summary>
        /// Grid border prefab.
        /// </summary>
        [SerializeField] private Image _borderPrefab;
        /// <summary>
        /// The size of for each cell in the sudoku grid view.
        /// </summary>
        [SerializeField] private float _cellSize = 100f;
        /// <summary>
        /// The thickness of the borders/lines of the sudoku grid view.
        /// </summary>
        [Range(1f, 10f)][SerializeField] private float _borderThickness = 6f;

        [SerializeField] private Color _selectedCellColor;
        [SerializeField] private Color _focusedCellColor;

        /// <summary>
        /// Cache the selected cell index in the grid.
        /// </summary>
        private (int row, int col) _selectedCell = (-1, -1);

        public SudokuGenerator _generator;

        /// <summary>
        /// Caches the rect transform component of this game object.
        /// </summary>
        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        /// <summary>
        /// Draws the sudoku grid and fills it with the current solution loaded in the sudoku results library.
        /// </summary>
        private void Start()
        {
            DrawSudoku();
            //FillNumbers(_sudokuResults.GetCurrentSolution());
        }

        /// <summary>
        /// Test methods...
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) _sudoku = _generator.Generate();
            if (Input.GetKeyDown(KeyCode.T)) FillNumbers(_sudoku.GetSolution());
            else if (Input.GetKeyDown(KeyCode.R)) FillNumbers(_sudoku.GetPuzzle());
        }

        #region Base Sudoku Grid Methods
        /// <summary>
        /// Draws the sudoku grid.
        /// </summary>
        public void DrawSudoku()
        {
            ClearGrid();
            DrawCells();
            DrawBorders();
        }

        /// <summary>
        /// Draws the sudoku grid cells.
        /// </summary>
        private void DrawCells()
        {
            RectTransform cells = new GameObject("Cells").AddComponent<RectTransform>();
            cells.SetParent(transform);
            cells.anchoredPosition = Vector2.zero;
            float gridTopLeftPos = _cellSize * 4.5f - _cellSize * 0.5f;

            for (int row = 0; row < 9; row++) {
                for (int col = 0; col < 9; col++) {
                    Vector3 position = new Vector3(-gridTopLeftPos + col * _cellSize, gridTopLeftPos - row * _cellSize, 0);
                    _grid[row, col] = Instantiate(_cellPrefab, cells);
                    _grid[row, col].RectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                    _grid[row, col].RectTransform.anchoredPosition = position;
                    _grid[row, col].Initialize();
                }
            }
        }

        /// <summary>
        /// Draws the sudoku grid broders/lines.
        /// </summary>
        private void DrawBorders()
        {
            RectTransform borders = new GameObject("Borders").AddComponent<RectTransform>();
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

            _rectTransform.sizeDelta = new Vector2(borderLength, borderLength);
        }

        /// <summary>
        /// Sets the sudoku to draw in this view.
        /// </summary>
        /// <param name="sudoku"></param>
        public void SetSudoku(Sudoku sudoku)
        {
            _sudoku = sudoku;
            FillSudokuGrid(sudoku.Puzzle);
        }

        public void FillSudokuGrid(int[,] puzzle)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _grid[row, col].SetNum(puzzle[row, col]);
        }

        /// <summary>
        /// Fills the sudoku grid with the given numbers/solution.
        /// </summary>
        /// <param name="digits">The sudoku solution.</param>
        public void FillNumbers(List<int> digits)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _grid[row, col].SetNum(digits[row * 9 + col]);
        }

        /// <summary>
        /// Clears the grid of all cells game objects.
        /// </summary>
        private void ClearGrid()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    if (_grid[row, col] == null) return;
                    Destroy(_grid[row, col].gameObject);
                }
        }
        #endregion

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
                    _grid[row, col].Select(_selectedCellColor, HighlightNeighbors);
                }
                else if (row == _selectedCell.row && col == _selectedCell.col) {
                    _grid[row, col].Deselect(HighlightNeighbors);
                    _selectedCell = (-1, -1);
                }
                else {
                    _grid[_selectedCell.row, _selectedCell.col].Deselect(HighlightNeighbors);
                    _selectedCell = (row, col);
                    _grid[row, col].Select(_selectedCellColor, HighlightNeighbors);
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
                if (i != _selectedCell.row) _grid[i, _selectedCell.col].Focus(_focusedCellColor, on);
                if (i != _selectedCell.col) _grid[_selectedCell.row, i].Focus(_focusedCellColor, on);

                int r = i / 3;
                int c = i % 3;
                if (boxRow + r != _selectedCell.row && boxCol + c != _selectedCell.col)
                    _grid[boxRow + r, boxCol + c].Focus(_focusedCellColor, on);

                for (int j = 0; j < 9; j++)
                    if (_grid[_selectedCell.row, _selectedCell.col].Number == _grid[i, j].Number
                     && _selectedCell.row != i
                     && _selectedCell.col != j
                     && _grid[_selectedCell.row, _selectedCell.col].Number != 0)
                        if (on) _grid[i, j].Select(_selectedCellColor, null);
                        else _grid[i, j].Deselect(null);
            }
        }
    }
}