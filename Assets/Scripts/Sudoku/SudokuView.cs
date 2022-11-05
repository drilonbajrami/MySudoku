using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SudokuTesting;

namespace MySudoku
{
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
            //if (Input.GetKeyDown(KeyCode.O)) foreach (Cell cell in _grid) cell.ToggleSums();

            if (Input.GetKeyDown(KeyCode.E)) _sudoku = _generator.Generate();
            //if (Input.GetKeyDown(KeyCode.T)) SetGridValues(_sudoku.Solution);
            //else if (Input.GetKeyDown(KeyCode.R)) SetGridValues(_sudoku.Puzzle);

            if (Input.GetKeyDown(KeyCode.Space)) {
                //_sudoku = new Sudoku();
                //_generator.GenerateSolution(_sudoku);
                //SetGridValues(_sudoku.Solution);
                //StartCoroutine(_generator.GeneratePuzzle(_sudoku, UpdateValues, waitSeconds, _grid, Difficulty.BEGGINER));
                RunGenerator();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                _sudoku = new Sudoku();
                //_sudoku.Puzzle.SetPuzzle("000020000768149235502060140210006904000000002809210300004053000000600000600472003");
                //_sudoku.Puzzle.SetPuzzle("004006020007800910000000308018300200300789001009001060803000500045003600026500100");
                //_sudoku.Puzzle.SetPuzzle("001957063000806070769130805007261350312495786056378000108609507090710608674583000");
                _sudoku.Puzzle.SetPuzzle("934060050006004923008900046800546007600010005500390062360401270470600500080000634");
                //_sudoku.Puzzle.SetPuzzle("009030600036014089100869035090000800010000090068090170601903002972640300003020900");
                _viewNotes.SetNotes(_sudoku.Puzzle);
                
                UpdateValues();
            }
            if(Input.GetKeyDown(KeyCode.A)) {
                if (_sudoku.Puzzle.DoublePairs(_viewNotes)) {
                    Debug.Log("Tried.");
                }
                //StartCoroutine(Try(_viewNotes, _grid));
                UpdateValues();
            }

            if (_selectedCell.row != -1 && _selectedCell.col != -1 && Input.anyKeyDown) {
                if (Input.GetKeyDown(KeyCode.Keypad1)) OnNumberClicked(1);
                if (Input.GetKeyDown(KeyCode.Keypad2)) OnNumberClicked(2);
                if (Input.GetKeyDown(KeyCode.Keypad3)) OnNumberClicked(3);
                if (Input.GetKeyDown(KeyCode.Keypad4)) OnNumberClicked(4);
                if (Input.GetKeyDown(KeyCode.Keypad5)) OnNumberClicked(5);
                if (Input.GetKeyDown(KeyCode.Keypad6)) OnNumberClicked(6);
                if (Input.GetKeyDown(KeyCode.Keypad7)) OnNumberClicked(7);
                if (Input.GetKeyDown(KeyCode.Keypad8)) OnNumberClicked(8); 
                if (Input.GetKeyDown(KeyCode.Keypad9)) OnNumberClicked(9);
                if (Input.GetKeyDown(KeyCode.Keypad0)) {
                    if (!noteToggle)
                        Set(_selectedCell.row, _selectedCell.col, 0); 
                }
            }
        }

        public IEnumerator Try(bool[,] notes, Cell[,] grid)
        {
            // Process only the diagonal boxes (1, 5, 9)
            for (int box = 0; box < 9; box += 3) {
                Debug.Log($"BOX ({box}, {box}):");

                // Check for each note.
                for (int i = 0; i < 9; i++) {

                    Debug.Log($"    Note {i + 1}:");

                    // Keep track of rows where this note is present in boxes.
                    // boxPerRow[x, y] -> x -> box index && y -> row index.
                    bool[,] boxPerRow = new bool[3, 3];

                    // Keep track of columns where this note is present in boxes.
                    // boxPerCol[x, y] -> x -> box index && y -> column index.
                    bool[,] boxPerCol = new bool[3, 3];

                    // d -> row or column index.
                    // k -> index of element in row or column.
                    for (int d = 0; d < 3; d++) {
                        for (int k = 0; k < 9; k++) {
                            Debug.Log($"        ROW ({d + box}, {k}) for {i + 1} => {notes[(d + box) * 9 + k, i]}");
                            //if (sudoku[d + box, k] == 0)
                            boxPerRow[k / 3, d] = boxPerRow[k / 3, d] || notes[(d + box) * 9 + k, i];

                            Debug.Log($"        COL ({k}, {d + box}) for {i + 1} => {notes[k * 9 + d + box, i]}");
                            //if (sudoku[k, d + box] == 0)
                            boxPerCol[k / 3, d] = boxPerCol[k / 3, d] || notes[k * 9 + d + box, i];

                            _grid[d + box, k].Focus(Color.green, true);
                            _grid[k, d + box].Focus(Color.green, true);
                            yield return new WaitForSeconds(0.025f);
                            _grid[d + box, k].Focus(Color.green, false);
                            _grid[k, d + box].Focus(Color.green, false);
                        }
                    }
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
            else Set(_selectedCell.row, _selectedCell.col, 9);
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
            _viewNotes.UpdateNotes(_sudoku.Puzzle, (row, col), _sudoku.Puzzle[row, col], num);
            _sudoku.Puzzle[row, col] = num;
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