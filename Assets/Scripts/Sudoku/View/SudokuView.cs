using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SudokuTesting;
using System.Reflection;

namespace MySudoku
{
    public class SudokuView : MonoBehaviour, IPointerClickHandler
    {
        public static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        #region Sudoku Fields
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
        #endregion

        #region View Settings
        [Header("Sudoku View Settings")]
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
        /// <summary>
        /// Color for a selected cell in the sudoku grid.
        /// </summary>
        [SerializeField] private Color _selectedCellColor;
        /// <summary>
        /// Color for focused cells in the sudoku grid.
        /// </summary>
        [SerializeField] private Color _focusedCellColor;
        #endregion

        /// <summary>
        /// Cache the selected cell index in the grid.
        /// </summary>
        private (int row, int col) _selectedCell = (-1, -1);

        [Space(10)]
        [Header("Other settings")]
        [SerializeField] private SudokuGenerator _generator;
        [SerializeField] private PermutationTableView _tableView;

        public bool _showPermutationGroups;
        public bool _calculateSums;

        public float waitSeconds = 1f;

        private readonly bool[,] _viewNotes = new bool[81, 9];

        public Toggle noteToggle;

        /// <summary>
        /// Caches the rect transform component of this game object.
        /// </summary>
        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        /// <summary>
        /// Draws the sudoku grid and fills it with the current solution loaded in the sudoku results library.
        /// </summary>
        private void Start() => DrawSudoku();

        /// <summary>
        /// Test methods...
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) _sudoku = _generator.Generate();
            //if (Input.GetKeyDown(KeyCode.T)) SetGridValues(_sudoku.Solution);
            //else if (Input.GetKeyDown(KeyCode.R)) SetGridValues(_sudoku.Puzzle);

            if (Input.GetKeyDown(KeyCode.Space)) {
                //_sudoku = new Sudoku();
                //_generator.GenerateSolution(_sudoku);
                //SetGridValues(_sudoku.Solution);
                //StartCoroutine(_generator.GeneratePuzzle(_sudoku, UpdateValues, waitSeconds, _grid, Difficulty.BEGGINER));
                ClearLog();
                RunGenerator();
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                bool[,] n = new bool[81, 9];
                Array.Copy(_viewNotes, n, _viewNotes.Length);
                _generator.TrySolve(_sudoku.Puzzle, _sudoku.Solution, n, logResult: true);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                _sudoku = new Sudoku();
                //_sudoku.Puzzle.SetPuzzle("032006100410000000000901000500090004060000071300020005000508000000000519057009860"); // Candidate Lines
                //_sudoku.Puzzle.SetPuzzle("400000938032094100095300240370609004529001673904703090957008300003900400240030709"); // Naked pairs
                //_sudoku.Puzzle.SetPuzzle("720408030080000047401076802810739000000851000000264080209680413340000008168943275"); // Hidden pairs
                //_sudoku.Puzzle.SetPuzzle("000030000020009016015000000100706040070500800000000001000000003050274000060010008");
                //_sudoku.Puzzle.SetPuzzle("801006094300009080970080500547062030632000050198375246083620915065198000219500008");
                //_sudoku.Puzzle.SetPuzzle("070408029002000004854020007008374200020000000003261700000093612200000403130642070");
                _sudoku.Puzzle.SetPuzzle("294513006600842319300697254000056000040080060000470000730164005900735001400928637"); // Naked Triples
                _sudoku.Puzzle.SetPuzzle("070408029002000004854020007008374200020000000003261700000093612200000403130642070");
                _sudoku.Puzzle.SetPuzzle("600802735702356940300407062100975024200183079079624003400560207067240300920738406");


                _viewNotes.SetNotes(_sudoku.Puzzle);
                
                UpdateValues();
            }
            if(Input.GetKeyDown(KeyCode.A)) {
                ISudokuTechnique technique = new NakedTriples {
                    LogConsole = true
                };
                if (technique.ApplyTechnique(_sudoku.Puzzle, _viewNotes))
                    UpdateValues();
                //StartCoroutine(Try(_viewNotes, _grid));
                //UpdateValues();
            }

            HandleInput();
        }

        private void HandleInput()
        {
            if (_selectedCell.row != -1 && _selectedCell.col != -1 && Input.anyKeyDown) {
                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) OnNumberClicked(1);
                if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) OnNumberClicked(2);
                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) OnNumberClicked(3);
                if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) OnNumberClicked(4);
                if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) OnNumberClicked(5);
                if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) OnNumberClicked(6);
                if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) OnNumberClicked(7);
                if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) OnNumberClicked(8);
                if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) OnNumberClicked(9);
                if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
                    if (!noteToggle)
                        Set(_selectedCell.row, _selectedCell.col, 0);
                }
            }
        }

        private void OnNumberClicked(int number)
        {
            if (noteToggle.isOn) {
                bool currentFlag = _viewNotes[_selectedCell.row * 9 + _selectedCell.col, number - 1];
                _viewNotes[_selectedCell.row * 9 + _selectedCell.col, number - 1] = !currentFlag;
                _grid[_selectedCell.row, _selectedCell.col].ShowNote(number, !currentFlag);
            }
            else Set(_selectedCell.row, _selectedCell.col, number);
        }

        public int sample = 1000;

        public void RunGenerator()
        {
            _sudoku = new Sudoku();
            for (int i = 0; i < sample; i++) {
                _sudoku.Solution = _generator.GenerateSolution();
                _sudoku.Puzzle = _generator.GeneratePuzzle(_sudoku.Solution, out bool[,] notes, out int ds);
                Array.Copy(notes, _viewNotes, notes.Length);
            }
            UpdateValues();
            _sudoku.PrintPuzzle();
        }

        private void Set(int row, int col, int num)
        {
            _sudoku.Puzzle[row, col] = num;
            _viewNotes.UpdateNotes(_sudoku.Puzzle, (row, col), _sudoku.Puzzle[row, col], num);     
            UpdateValues();
        }

        #region Sudoku Draw Methods
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
        /// Sets the sudoku to draw in this view.
        /// </summary>
        /// <param name="sudoku"></param>
        public void SetSudoku(Sudoku sudoku)
        {
            _sudoku = sudoku;
            SetGridValues(sudoku.Puzzle);
        }

        /// <summary>
        /// Sets the grid values based on the given sudoku grid.
        /// </summary>
        /// <param name="sudokuGrid">The sudoku grid to get the values from.</param>
        public void SetGridValues(int[,] sudokuGrid)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _grid[row, col].SetNum(sudokuGrid[row, col]);

            //_tableView.Initialize(_grid, _showPermutationGroups);
            //if (_calculateSums) Sudokutils.CalculateSums(_grid);
        }

        public void UpdateValues()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    _grid[row, col].SetNum(_sudoku.Puzzle[row, col]);
                    CheckNotes((row, col));
                }
        }

        private void CheckNotes((int row, int col) cell)
        {
            for (int n = 0; n < 9; n++)
                _grid[cell.row, cell.col].ShowNote(n + 1, _viewNotes[cell.row * 9 + cell.col, n]);
        }

        #region Click Handling Methods
        /// <summary>
        /// Handles clicks on the sudoku grid.
        /// </summary>
        /// <param name="eventData">Pointer event data to process.</param>
        public void OnPointerClick(PointerEventData eventData)
            => HandleClickedCell(GetClickedCellIndex(eventData.pressPosition));

        /// <summary>
        /// Gets the clicked cell row and column index.
        /// </summary>
        /// <param name="pressPosition">Mouse press position.</param>
        /// <returns>Row and column index.</returns>
        private (int row, int col) GetClickedCellIndex(Vector2 pressPosition)
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
        private void HandleClickedCell((int row, int col) index)
        {
            int row = index.row;
            int col = index.col;

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
        #endregion
    }
}