using System;
using System.Collections.Generic;
using System.Text;
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

        [Range(30, 360)]
        public float maxGenerationDurationInSeconds = 60f;

        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.< /returns>
        public Sudoku Generate(Difficulty difficulty)
        {
            if (_randGenerator == null) {
                Debug.LogWarning("Random generator does not exist! Please add one.");
                return null;
            }

            Sudoku sudoku = new();
            _randGenerator.SetSeed(_seed);
            sudoku.Solution = GenerateSolution();
            sudoku.Puzzle = GeneratePuzzle(sudoku.Solution, difficulty);
            if (sudoku.Puzzle == null) return null;
            return sudoku;
        }

        public string GetPuzzle(int[,] puzzle)
        {
            StringBuilder puz = new StringBuilder();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    puz.Append($"{puzzle[row, col]}");
            return puz.ToString();
        }

        #region Puzzle Generator
        /// <summary>
        /// Generates a sudoku puzzle based on the given sudoku solution.
        /// </summary>
        /// <param name="solution">The sudoku solution.</param>
        /// <param name="notes">Notes for the generated sudoku puzzle.</param>
        /// <param name="difficultyScore">Overall difficulty score of the generated sudoku puzzle.</param>
        /// <returns>Sudoku puzzle.</returns>
        public int[,] GeneratePuzzle(int[,] solution, Difficulty difficulty)
        {
#if UNITY_EDITOR
            ConsoleLog.Clear();
#endif
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Debug.Log($"Started generation at: {DateTime.Now}");

            start:
            int[,] puzzle = new int[9, 9];
            bool[,] notes = new bool[81, 9];
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
            Queue<(int, int)> ind = new(indexes);

            int difficultyScore = 0;
            (int lower, int upper) difficultyRange = SudokuTechniques.DifficultyMap[difficulty];
            int difficultyCap = _randGenerator.Next(difficultyRange.lower, difficultyRange.upper);
            
            int tries = 40;
            List<(int row, int col)> emptyCellIndexes = new();

            while (difficultyScore < difficultyCap && tries > 0) {
                (int row, int col) index = ind.Dequeue();
                int oldValue = puzzle[index.row, index.col];

                // Set the current cell value to 0 and update notes.
                puzzle[index.row, index.col] = 0;
                notes.Update(puzzle, index, oldValue, 0);
                Array.Copy(notes, notesCopy, notes.Length);

                if (IsUnique(puzzle) && TrySolve(puzzle, solution, notesCopy, logResult: false, out int cost) && cost < difficultyRange.upper) {
                    difficultyScore = cost;
                }
                else {
                    ind.Enqueue(index);
                    puzzle[index.row, index.col] = oldValue;
                    notes.Update(puzzle, index, 0, oldValue);
                    tries--;
                }
            }

            if (stopwatch.ElapsedMilliseconds / 1000f > maxGenerationDurationInSeconds) {
                Debug.Log("Took too long to generate puzzle");
                return null;
            }

            if(difficultyScore < difficultyCap) goto start;

            Array.Copy(notes, notesCopy, notes.Length);
            TrySolve(puzzle, solution, notesCopy, logResult: false, out int diff);
            //if (SudokuTechniques.Techniques[(int)Technique.Hidden_Triples].TimesUsed == 0) goto start;

            #if UNITY_EDITOR
            //ConsoleLog.Clear();
            Debug.Log($"Selected difficulty: {difficulty} with score range [{difficultyRange.lower}, {difficultyRange.upper}] and score cap {difficultyCap}");
            Debug.Log($"Difficulty Score: {difficultyScore}");
            Debug.Log("Techniques: ");
            for (int i = 0; i < SudokuTechniques.Techniques.Count; i++)
                Debug.Log($"    {(Technique)i} used {SudokuTechniques.Techniques[i].TimesUsed} times.");
            GetPuzzle(puzzle).CopyToClipboard();
            stopwatch.Stop();
            Debug.Log($"Sduoku puzzle generated in {stopwatch.ElapsedMilliseconds / 1000f} seconds");
            #endif

            return puzzle;
        }

        /// <summary>
        /// Tries to solve a sudoku puzzle with currently available sudoku techniques.
        /// </summary>
        /// <param name="puzzleTemplate">The sudoku puzzle.</param>
        /// <param name="solution">The sudoku solution.</param>
        /// <param name="notesCopy">The notes for the sudoku puzzle.</param>
        /// <returns>Whether this sudoku puzzle can be solved with currently available techniques.</returns>
        public bool TrySolve(int[,] puzzleTemplate, int[,] solution, bool[,] notesCopy, bool logResult, out int difficultyCost)
        {
            int[,] puzzle = new int[9, 9];
            Array.Copy(puzzleTemplate, puzzle, puzzleTemplate.Length);
            SudokuTechniques.ResetTechniqueUsageCount();

            bool solving = true;
            difficultyCost = 0;
            int cost;

            while (solving) {
                cost = puzzle.ApplyTechniques(notesCopy);
                solving = cost != 0;
                if (solving) difficultyCost += cost;
                if (puzzle.IsIdenticalTo(solution)) {
                    if (logResult) Debug.Log($"Solved with difficulty cost of {difficultyCost}.");
                    return true;
                };
            }
           
            return false;
        }

        /// <summary>
        /// Checks if a sudoku puzzle has a unique solution or not.
        /// </summary>
        /// <param name="sudoku">Sudoku puzzle.</param>
        /// <returns>Whether this sudoku is unique or not.</returns>
        private bool IsUnique(int[,] sudoku) => Solve(sudoku) == 1;

        /// <summary>
        /// Returns the number of available solutions for a sudoku puzzle.
        /// </summary>
        /// <param name="sudoku">Sudoku puzzle to solve.</param>
        /// <param name="row">Current row.</param>
        /// <param name="col">Current column.</param>
        /// <param name="count">Number of solutions.</param>
        /// <returns>Number of solutions for this sudoku puzzle.</returns>
        private int Solve(int[,] sudoku, int row = 0, int col = 0, int count = 0)
        {
            if (col == 9) {
                col = 0;
                if (++row == 9) return 1 + count;
            }

            // Skip cells that are not empty.
            if (sudoku[row, col] != 0) return Solve(sudoku, row, col + 1, count);

            // Search for 2 solutions instead of 1.
            // Break, if 2 solutions are found
            for (int val = 1; val <= 9 && count < 2; ++val) 
                if (sudoku.CanUseNumber(row, col, val)) {
                    sudoku[row, col] = val;
                    // Add additional solutions if possible.
                    count = Solve(sudoku, row, col + 1, count);
                }

            // Reset on backtrack.
            sudoku[row, col] = 0; 
            return count;
        }
        #endregion

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