using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySudoku
{
    /// <summary>
    /// Class for sudoku extension methods.
    /// </summary>
    public static class SudokuExtension
    {
        /// <summary>
        /// Check if the given number can be placed in the specified row and column of the sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku grid.</param>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="num">The number to check for.</param>
        /// <returns>Whether the given number can be used or not, in the given row and column of the sudoku.</returns>
        public static bool CanUseNumber(this int[,] sudoku, int row, int col, int num)
            => !sudoku.HasNumberInRow(row, num)
            && !sudoku.HasNumberInColumn(col, num)
            && !sudoku.HasNumberInBox(row, col, num);

        /// <summary>
        /// Checks if this sudoku grid contains the given number in the specified box.<br/>
        /// The box is calculated through the given row and column.
        /// </summary>
        /// <param name="sudoku">The sudoku grid.</param>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <param name="num">The number to check for.</param>
        /// <returns>Whether the given number is present or not, in the box of the sudoku.</returns>
        public static bool HasNumberInBox(this int[,] sudoku, int row, int col, int num)
        {
            int boxRow = row - row % 3;
            int boxCol = col - col % 3;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (sudoku[boxRow + r, boxCol + c] == num) return true;

            return false;
        }

        /// <summary>
        /// Checks if this sudoku grid contains the given number in the given column.
        /// </summary>
        /// <param name="sudoku">The sudoku grid.</param>
        /// <param name="col">The column.</param>
        /// <param name="num">The number to check for.</param>
        /// <returns>Whether the given number is present or not, in the given column of the sudoku.</returns>
        public static bool HasNumberInColumn(this int[,] sudoku, int col, int num)
        {
            for (int row = 0; row < 9; row++)
                if (sudoku[row, col] == num) return true;

            return false;
        }

        /// <summary>
        /// Checks if this sudoku grid contains the given number in the given row.
        /// </summary>
        /// <param name="sudoku">The sudoku grid.</param>
        /// <param name="row">The row.</param>
        /// <param name="num">The number to check for.</param>
        /// <returns>Whether the given number is present or not, in the given row of the sudoku.</returns>
        public static bool HasNumberInRow(this int[,] sudoku, int row, int num)
        {
            for (int col = 0; col < 9; col++)
                if (sudoku[row, col] == num) return true;

            return false;
        }

        /// <summary>
        /// Checks if this sudoku puzzle is identical to the given sudoku solution.
        /// </summary>
        /// <param name="puzzle">The sudoku puzzle.</param>
        /// <param name="solution">The sudoku solution to compare to.</param>
        /// <returns>Whether the puzzle and solution are equal or not.</returns>
        public static bool IsIdenticalTo(this int[,] puzzle, int[,] solution)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    if (puzzle[row, col] != solution[row, col])
                        return false;

            return true;
        }

        /// <summary>
        /// Shuffles this list with a <see cref="RandomGenerator"/> which has a seed.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="list">List to shuffle.</param>
        /// <param name="random">Random generator.</param>
        public static void Shuffle<T>(this List<T> list, RandomGenerator random)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static void SetPuzzle(this int[,] grid, string puzzle)
        {
            for (int row = 0; row < 9; row++) {
                for (int col = 0; col < 9; col++) {
                    grid[row, col] = (int)char.GetNumericValue(puzzle[row * 9 + col]);
                }
            }
        }

        public static string CopyToClipboard(this int[,] puzzle)
        {
            StringBuilder puz = new StringBuilder();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    puz.Append($"{puzzle[row, col]}");

            string p = puz.ToString();
            p.CopyToClipboard();
            return p;
        }
    }
}