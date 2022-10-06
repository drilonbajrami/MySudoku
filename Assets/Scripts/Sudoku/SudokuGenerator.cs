using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro.EditorUtilities;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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

            _randGenerator.SetSeed(_seed);
            Sudoku sudoku = new();
            GenerateSolution(sudoku);
            GeneratePuzzle(sudoku, 30);
            return sudoku;
        }

        /// <summary>
        /// Generates the solution for the given sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku to generate the solution for.</param>
        private void GenerateSolution(Sudoku sudoku)
        {
            // Fill the diagonal boxes first (1,5,9) because these boxes
            // do not affect each other's order of numbers.
            for (int i = 0; i < 9; i += 3)
                FillBox(sudoku.Solution, row: i, col: i);

            // Fills the remaining cells in boxes (2,3,4,6,7,8).
            FillRemaining(sudoku.Solution, row: 0, col: 3);
        }

        /// <summary>
        /// Generates the puzzle for the given sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku to generate the puzzle for.</param>
        private void GeneratePuzzle(Sudoku sudoku) // To be changed
        {
            Array.Copy(sudoku.Solution, sudoku.Puzzle, sudoku.Solution.Length);
            int count = 64;
            while (count != 0) {
                // Pick random cell
                int cellId = _randGenerator.Next(81);

                // extract coordinates i and j
                int row = cellId / 9;
                int col = cellId % 9;
                if (col != 0)
                    col--;

                // If not removed yet, then remove
                if (sudoku.Puzzle[row, col] != 0) {
                    count--;
                    sudoku.Puzzle[row, col] = 0;
                }
            }
        }
        
        private void GeneratePuzzle(Sudoku sudoku, int numOfClues)
        {
            Array.Copy(sudoku.Solution, sudoku.Puzzle, sudoku.Solution.Length);
            //int clueCount = 81;

            List<(int row, int col)> indexes = new();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    indexes.Add((row, col));
                }

            for (int i = 0; i < 15; i++) {
                int randIndex = _randGenerator.Next(0, indexes.Count);
                (int row, int col) randomCell = indexes[randIndex];
                indexes.RemoveAt(randIndex);
                sudoku.Puzzle[randomCell.row, randomCell.col] = 0;
                if (randomCell.row != 5 || randomCell.col != 5) {
                    indexes.Remove((8 - randomCell.row, 8 - randomCell.col));
                    sudoku.Puzzle[8 - randomCell.row, 8 - randomCell.col] = 0;
                }
            }
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
    }
}