using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    [RequireComponent(typeof(RandomGenerator))]
    public class SolutionGenerator : MonoBehaviour
    {
        /// <summary>
        /// Seed for the random generator.
        /// </summary>
        [SerializeField] private int _seed;

        /// <summary>
        /// Random number generator with seed.
        /// </summary>
        [SerializeField] RandomGenerator _random;

        /// <summary>
        /// Generates the solution for the given sudoku.
        /// </summary>
        public int[,] Generate()
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
                        int index = _random.Next(numbers.Count);
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

        public void SetRandomGenerator(RandomGenerator random) => _random = random;

        /// <summary>
        /// Sets the seed and uses own random generator for the solution generator.
        /// </summary>
        public void SetSeed(int seed)
        {
            _seed = seed;
            _random = GetComponent<RandomGenerator>();
            _random.SetSeed(_seed);
        }
    }
}