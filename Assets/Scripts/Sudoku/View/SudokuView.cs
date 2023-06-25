using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using Unity.Mathematics;

namespace MySudoku
{

    /* Grid Dimensions Template
	- 4 (tb) thick borders
	- 6 (sb) slim borders
	- 9 (cs) cell size

    Row & Col spacing:
	    x0 = tb + 1/2 * cs
	    x1 = tb + sb + 3/2 * cs
	    x2 = tb + 2sb + 5/2 * cs

	    x3 = 2tb + 2sb + 7/2 * cs
	    x4 = 2tb + 3sb + 9/2 * cs
	    x5 = 2tb + 4sb + 11/2 * cs

	    x6 = 3tb + 4sb + 13/2 * cs
	    x7 = 3tb + 5sb + 15/2 * cs
	    x8 = 3tb + 6sb + 17/2 * cs
 */

    public class SudokuView : MonoBehaviour, IPointerClickHandler
    {
        #region Sudoku Fields
        /// <summary>
        /// The sudoku to show/draw.
        /// </summary>
        private Sudoku _sudoku;

        /// <summary>
        /// Grid of cells.
        /// </summary>
        private readonly Cell[,] _grid = new Cell[9, 9];

        private int[,] _puzzleCopy = new int[9, 9];

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

        private float _totalGridSize;
        [SerializeField] private float _cellSize = 100f;
        [SerializeField] private float _thickBorder = 12f;
        [SerializeField] private float _slimBorder = 3f;

        private float _cachedCellSize;
        private float _cachedThickBorder;
        private float _cachedSlimBorder;

        public bool LayoutSettingsChanged { get; private set; } = false;

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

        private readonly bool[,] _viewNotes = new bool[81, 9];

        public Toggle noteToggle;
        public GameObject _onText;
        public GameObject _offText;

        public Difficulty diff;

        [SerializeField] string _currentPuzzle;

        public string enterPuzzle;

        /// <summary>
        /// Caches the rect transform component of this game object.
        /// </summary>
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _cachedCellSize = _cellSize;
            _cachedThickBorder = _thickBorder;
            _cachedSlimBorder = _slimBorder;
            DrawSudoku();
        }

        /// <summary>
        /// Draws the sudoku grid and fills it with the current solution loaded in the sudoku results library.
        /// </summary>
        //private void Start() => DrawSudoku();

        /// <summary>
        /// Test methods...
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                noteToggle.isOn = !noteToggle.isOn;

            if (Input.GetKeyDown(KeyCode.A)) {
                ISudokuTechnique technique = new HiddenTriples {
                    LogConsole = true
                };
                if (technique.ApplyTechnique(_puzzleCopy, _viewNotes, out int cost)) {
                    UpdateGridViewNotes();
                }
            }

            HandleInput();
        }

        public void GenerateBeginnerSudoku() => SetSudoku(_generator.Generate(Difficulty.Beginner));
        public void GenerateEasySudoku() => SetSudoku(_generator.Generate(Difficulty.Easy));
        public void GenerateMediumSudoku() => SetSudoku(_generator.Generate(Difficulty.Medium));
        public void GenerateHardSudoku() => SetSudoku(_generator.Generate(Difficulty.Hard));
        public void GenerateExtremeSudoku() => SetSudoku(_generator.Generate(Difficulty.Extreme));
        public void GenerateEvilSudoku() => SetSudoku(_generator.Generate(Difficulty.Evil));

        public void OnNoteEditToggleClicked(bool on)
        {
            _onText.SetActive(on);
            _offText.SetActive(!on);
        }

        /// <summary>
        /// Sets the sudoku to draw in this view.
        /// </summary>
        /// <param name="sudoku"></param>
        public void SetSudoku(Sudoku sudoku)
        {
            if (sudoku == null) return;
            _sudoku = sudoku;
            _currentPuzzle = _sudoku.PrintPuzzle();
            ResetPuzzle();
        }

        public void ResetPuzzle()
        {
            Array.Copy(_sudoku.Puzzle, _puzzleCopy, _sudoku.Puzzle.Length);
            UpdateGridViewValues();
        }

        /// <summary>
        /// Sets the grid values based on the given sudoku puzzle.
        /// </summary>
        public void UpdateGridViewValues()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    _grid[row, col].SetNum(_puzzleCopy[row, col]);
                    for (int n = 0; n < 9; n++) {
                        _grid[row, col].ShowNote(n + 1, false);
                        _viewNotes[row * 9 + col, n] = false;
                    }
                }
        }

        public void UpdateGridViewNotes()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    for (int n = 0; n < 9; n++)
                        _grid[row, col].ShowNote(n + 1, _viewNotes[row * 9 + col, n]);
                }
        }

        public void FillAndUpdateNotes()
        {
            _viewNotes.SetNotes(_puzzleCopy);
            UpdateGridViewNotes();
        }

        public void ClearAllNotes()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    for (int n = 0; n < 9; n++) {
                        _viewNotes[row * 9 + col, n] = false;
                        _grid[row, col].ShowNote(n + 1, false);
                    }
                }
        }

        /// <summary>
        /// Sets cell value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="num"></param>
        private void Set(int row, int col, int num)
        {
            if (_puzzleCopy[row, col] == num) return;
            _puzzleCopy[row, col] = num;
            _viewNotes.UpdateOnCellEdit((row, col), num);
            _grid[row, col].SetNum(num);
            UpdateGridViewNotes();
        }

        public void EnterPuzzle()
        {
            if (enterPuzzle.Length == 81 && Regex.IsMatch(enterPuzzle, @"^\d+$")) {
                Sudoku s = new Sudoku();
                s.Puzzle.SetPuzzle(enterPuzzle);
                SetSudoku(s);
                FillAndUpdateNotes();
            }
            else
                Debug.LogWarning("Could not enter the given puzzle. The given puzzle does not contain 81 characters and/or not all characters are numbers.");
        }

        public void CopyPuzzleToClipboard() => _currentPuzzle.CopyToClipboard();
        
        #region Sudoku Draw Methods
        /// <summary>
        /// Draws the sudoku grid.
        /// </summary>
        public void DrawSudoku()
        {
            _totalGridSize = 4 * _thickBorder + 6 * _slimBorder + 9 * _cellSize;
            _rectTransform.sizeDelta = new Vector2(_totalGridSize, _totalGridSize);

            for (int row = 0; row < 9; row++) {
                for (int col = 0; col < 9; col++) {
                    float x = (col / 3 + 1) * _thickBorder + (col % 3 + col / 3 * 2) * _slimBorder + (col + 0.5f) * _cellSize;
                    float y = (row / 3 + 1) * _thickBorder + (row % 3 + row / 3 * 2) * _slimBorder + (row + 0.5f) * _cellSize;
                    Vector2 position = new Vector3(x - _totalGridSize / 2f, _totalGridSize / 2f - y);

                    if (_grid[row, col] == null) {
                        _grid[row, col] = Instantiate(_cellPrefab, transform);
                        _grid[row, col].Index = (row, col);
                        _grid[row, col].OnClicked += OnCellClicked;
                    }

                    _grid[row, col].RectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                    _grid[row, col].RectTransform.anchoredPosition = position;
                    _grid[row, col].Initialize();
                }
            }

            _cachedCellSize = _cellSize;
            _cachedThickBorder = _thickBorder;
            _cachedSlimBorder = _slimBorder;
            LayoutSettingsChanged = false;
        }

        public void OnValidate()
            => LayoutSettingsChanged = _cachedCellSize != _cellSize || 
                                       _cachedThickBorder != _thickBorder || 
                                       _cachedSlimBorder != _slimBorder;
        
        #endregion

        #region Click Handling Methods

        private void OnCellClicked((int row, int col) index)
        {
            _selectedCell = index;
            Debug.Log($"Clicked Cell - [{index.row},{index.col}]");
        }

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
                if (i != _selectedCell.row) _grid[i, _selectedCell.col].SetFocus(_focusedCellColor, on);
                if (i != _selectedCell.col) _grid[_selectedCell.row, i].SetFocus(_focusedCellColor, on);

                int r = i / 3;
                int c = i % 3;
                if (boxRow + r != _selectedCell.row && boxCol + c != _selectedCell.col)
                    _grid[boxRow + r, boxCol + c].SetFocus(_focusedCellColor, on);

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

        #region Keyboard Input Handling
        /// <summary>
        /// Handles input for any number key pressed (Keypad or Alpha number key).
        /// </summary>
        private void HandleInput()
        {
            if (_selectedCell.row != -1 && _selectedCell.col != -1 && Input.anyKeyDown) {
                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) OnNumberKeyPressed(1);
                if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) OnNumberKeyPressed(2);
                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) OnNumberKeyPressed(3);
                if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) OnNumberKeyPressed(4);
                if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) OnNumberKeyPressed(5);
                if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)) OnNumberKeyPressed(6);
                if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) OnNumberKeyPressed(7);
                if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) OnNumberKeyPressed(8);
                if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) OnNumberKeyPressed(9);
                if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Backspace)) OnNumberKeyPressed(0);
            }
        }

        /// <summary>
        /// Updates the cell's value to the pressed key number if there is a selected cell.
        /// </summary>
        /// <param name="number">Number to set.</param>
        private void OnNumberKeyPressed(int number)
        {
            if (noteToggle.isOn && number != 0) {
                // If cell is already filled then do nothing.
                if (_puzzleCopy[_selectedCell.row, _selectedCell.col] != 0) return;

                // Toggle note.
                bool currentFlag = _viewNotes[_selectedCell.row * 9 + _selectedCell.col, number - 1];
                _viewNotes[_selectedCell.row * 9 + _selectedCell.col, number - 1] = !currentFlag;
                _grid[_selectedCell.row, _selectedCell.col].ShowNote(number, !currentFlag);
            }
            else/* if (_sudoku.Puzzle[_selectedCell.row, _selectedCell.col] == 0)*/ {
                Set(_selectedCell.row, _selectedCell.col, number);
            }
        }
        #endregion
    }
}