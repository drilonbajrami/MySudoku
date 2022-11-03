using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Class for generating a sudoku with a solution and puzzle.
    /// </summary>
    public class SudokuGenerator : MonoBehaviour
    {
        /// <summary>
        /// Seed for the random generator.
        /// </summary>
        [SerializeField] private int _seed;

        /// <summary>
        /// Random number generator with seed.
        /// </summary>
        [SerializeField] RandomGenerator _randGenerator;

        public string sol = "";

        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.< /returns>
        public Sudoku Generate()
        {
            if (_randGenerator == null) {
                Debug.LogWarning("Random generator does not exist! Please add one.");
                return null;
            }

            Sudoku sudoku = new();
            _randGenerator.SetSeed(_seed);
            sudoku.Solution = GenerateSolution();
            sudoku.Puzzle = GeneratePuzzle(sudoku.Solution, out bool[,] notes, out int ds);
            return sudoku;
        }

        public int[,] GeneratePuzzle(int[,] solution, out bool[,] notes, out int difficultyScore)
        {
            //startPoint:
            int[,] puzzle = new int[9, 9];
            notes = new bool[81, 9];
            bool[,] notesCopy = new bool[81, 9];

            Array.Copy(solution, puzzle, solution.Length);

            // Cache all sudoku grid cells by their indexes and clear the notes.
            List<(int row, int col)> indexes = new();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    indexes.Add((row, col));
                    for (int i = 0; i < 9; i++)
                        notes[row * 9 + col, i] = notesCopy[row * 9 + col, i] = false;
                }
            indexes.Shuffle(_randGenerator);
            Queue<(int,int)> ind = new(indexes);

            difficultyScore = 0;
            (int lower, int upper) difficultyScoreRange = SudokuTechniques.DifficultyMap[Difficulty.MEDIUM];

            int tries = 243;

            List<(int row, int col)> emptyCellIndexes = new();

            while (/*(difficultyScore < difficultyScoreRange.upper || */tries > 0) {
                (int row, int col) index = /*indexes[^1]*/ind.Dequeue();
                //indexes.RemoveAt(indexes.Count - 1);
                int oldValue = puzzle[index.row, index.col];

                // Set the current cell value to 0 and update notes.
                puzzle[index.row, index.col] = 0;
                notes.UpdateNotes(puzzle, index, oldValue, 0);
                Array.Copy(notes, notesCopy, notes.Length);

                if ((Solve(0, 0, puzzle) == 1) && TrySolve(puzzle, solution, notesCopy)) {
                    difficultyScore += 100;
                }
                else {
                    ind.Enqueue(index);
                    puzzle[index.row, index.col] = oldValue;
                    notes.UpdateNotes(puzzle, index, 0, oldValue);
                    tries--;
                }
            }

            //if(Solve(0,0, puzzle, 0) != 1) {
            //    Debug.Log("Trying again...");
            //    goto startPoint;
            //}

            Debug.Log($"Difficulty Score: {difficultyScore}");
            /*sol = */GetPuzzle(puzzle).CopyToClipboard();
            //sol.CopyToClipboard();
            return puzzle;
        }

        public string GetPuzzle(int[,] puzzle)
        {
            StringBuilder puz = new StringBuilder();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    puz.Append($"{puzzle[row, col]}");
            return puz.ToString();
        }

        private bool TrySolve(int[,] puzzleTemplate, int[,] solution, bool[,] notesCopy)
        {
            int[,] puzzle = new int[9, 9];
            Array.Copy(puzzleTemplate, puzzle, puzzleTemplate.Length);

            bool solved = false;
            bool solving = true;

            while (solving) {
                solving = puzzle.ApplyTechniques(notesCopy);
                solved = puzzle.IsIdenticalTo(solution);
            }

            return solved;
        }

        public bool SolveSudoku(int[,] grid, int row, int col)
        {
            if (row == 8 && col == 9)
                return true;

            if (col == 9) {
                row++;
                col = 0;
            }

            if (grid[row, col] != 0)
                return SolveSudoku(grid, row, col + 1);

            for (int num = 1; num < 10; num++) {
                if (grid.CanUseNumber(row, col, num)) {
                    grid[row, col] = num;
                    if (SolveSudoku(grid, row, col + 1))
                        return true;
                }
                grid[row, col] = 0;
            }

            return false;
        }

        // returns 0, 1 or more than 1 depending on whether 0, 1 or more than 1 solutions are found
        public int Solve(int row, int col, int[,] cells, int count = 0)
        {
            if (col == 9) {
                col = 0;
                if (++row == 9)
                    return 1 + count;
            }
            if (cells[row, col] != 0)  // skip filled cells
                return Solve(row, col + 1, cells, count);

            // search for 2 solutions instead of 1
            // break, if 2 solutions are found
            for (int val = 1; val <= 9 && count < 2; ++val) {
                if (cells.CanUseNumber(row, col, val)) {
                    cells[row, col] = val;
                    // add additional solutions
                    count = Solve(row, col + 1, cells, count);
                }
            }

            cells[row, col] = 0; // reset on backtrack
            return count;
        }

        /// <summary>
        /// Coroutine Sudoku Puzzle Generator
        /// </summary>
        //public IEnumerator GeneratePuzzle(Sudoku sudoku, Action action, float waitSeconds, Cell[,] grid, Difficulty difficulty)
        //{
        //    Array.Copy(sudoku.Solution, sudoku.Puzzle, sudoku.Solution.Length);

        //    // Cache all sudoku grid cells by their indexes
        //    List<(int row, int col)> cellIndexes = new();
        //    for (int row = 0; row < 9; row++)
        //        for (int col = 0; col < 9; col++) {
        //            cellIndexes.Add((row, col));
        //        }

        //    int difficultyScore = 0;
        //    (int lower, int upper) diffRange = SudokuTechniques.DifficultyMap[difficulty];
        //    bool solvable = true;

        //    int tries = 500;
        //    int lastStableCount = 500;

        //    while (/*difficultyScore < diffRange.upper ||*/  solvable || tries > 0) {
        //        // Pick random index (cell) and store its current value
        //        int randIndex = _randGenerator.Next(0, cellIndexes.Count);
        //        (int row, int col) cellIndex = cellIndexes[randIndex];

        //        //sudoku.SetValue(cellIndex, 0);

        //        // Highlight the cells for visualization purposes and invoke ACTION
        //        grid[cellIndex.row, cellIndex.col].Select(Color.magenta, null);
        //        yield return waitSeconds != 0 ? new WaitForSeconds(waitSeconds * 2f / 3f) : (object)null;

        //        action?.Invoke();

        //        if (TrySolve(sudoku)) {
        //            cellIndexes.RemoveAt(randIndex);
        //            difficultyScore += 100;
        //            //Debug.Log($"Difficulty now is at: {difficultyScore}");
        //            grid[cellIndex.row, cellIndex.col].Select(Color.green, null);
        //            lastStableCount = tries;
        //            Debug.Log($"Number of tries: {tries}        STABLE");
        //        }
        //        else {
        //            sudoku.SetValue(cellIndex, sudoku.Solution[cellIndex.row, cellIndex.col]);
        //            grid[cellIndex.row, cellIndex.col].Select(Color.red, null);
        //            solvable = false;
        //            tries--;
        //            Debug.Log($"Number of tries: {tries}");
        //        }

        //        // Highlight the cells for visualization purposes and invoke ACTION
        //        action?.Invoke();

        //        yield return waitSeconds != 0 ? new WaitForSeconds(waitSeconds / 3f) : (object)null;

        //        grid[cellIndex.row, cellIndex.col].Deselect(null);
        //    }

        //    Debug.Log($"Latest stable appearance at {lastStableCount}");
        //    Debug.Log($"Difficulty score: {difficultyScore}");
        //    sudoku.PrintPuzzle();
        //}

        #region Solution Generator
        /// <summary>
        /// Generates the solution for the given sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku to generate the solution for.</param>
        public int[,] GenerateSolution()
        {
            int[,] sudoku = new int[9, 9];
            // Fill the diagonal boxes first (1,5,9) because these boxes
            // do not affect each other's order of numbers.
            for (int i = 0; i < 9; i += 3)
                FillBox(sudoku, row: i, col: i);

            // Fills the remaining cells in boxes (2,3,4,6,7,8).
            FillRemaining(sudoku, row: 0, col: 3);

            return sudoku;
        }

        /// <summary>
        /// Fills the given box of sudoku grid with randomly placed numbers, from 1 to 9.
        /// </summary>
        /// <param name="sudoku">The sudoku solution grid.</param>
        /// <param name="row">The first row of the box.</param>
        /// <param name="col">The first column of the box.</param>
        private void FillBox(int[,] sudoku, int row, int col)
        {
            // Store the numbers from 1 to 9, so the same number does not get picked more than once, randomly.
            List<int> numbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int number;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++) {
                    do {
                        int index = _randGenerator.Next(numbers.Count);
                        number = numbers[index];
                        numbers.RemoveAt(index);
                    }
                    while (sudoku.HasNumberInBox(row, col, number));
                    sudoku[row + r, col + c] = number;
                }
        }

        /// <summary>
        /// Fills the remaining empty cells of the given sudoku grid, recursively.
        /// </summary>
        /// <param name="sudoku">The sudoku solution grid.</param>
        /// <param name="row">The row of the empty cell to fill.</param>
        /// <param name="col">The column of the empty cell to fill.</param>
        /// <returns>Whether this recursive function can go forward or backwards, continue or stop.</returns>
        private bool FillRemaining(int[,] sudoku, int row, int col)
        {
            // Got to the next row if the current row end is reached.
            if (col > 8 && row < 8) { row++; col = 0; }

            // Clamp the row and column:
            // Box 2 & 3.
            if (row < 3) { if (col < 3) col = 3; }
            // Box 4 & 6.
            else if (row < 6) { if (col == row / 3 * 3) col += 3; }
            // Box 7 & 8.
            else { if (col == 6) { row++; col = 0; if (row > 8) return true; } }

            for (int number = 1; number <= 9; number++)
                if (sudoku.CanUseNumber(row, col, number)) {
                    sudoku[row, col] = number;
                    // Go to the next cell, if true then go back one step recursively.
                    if (FillRemaining(sudoku, row, col + 1)) return true;
                    // Reset cell number to 0 (null/empty) if the current assigned number is not valid.
                    sudoku[row, col] = 0;
                }

            return false;
        }
        #endregion
    }
}