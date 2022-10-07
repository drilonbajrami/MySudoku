using System.Collections.Generic;
using System.Text;

namespace MySudoku
{
    /// <summary>
    /// Sudoku class that stores a sudoku puzzle and its solution.
    /// </summary>
    public class Sudoku
    {
        /// <summary>
        /// Sudoku puzzle.
        /// </summary>
        public int[,] Puzzle {
            get { return _puzzle; }
            set { _puzzle = value; }
        }

        /// <summary>
        /// Sudoku solution.
        /// </summary>
        public int[,] Solution {
            get { return _solution; }
            set { _solution = value; }
        }

        /// <summary>
        /// Puzzle.
        /// </summary>
        private int[,] _puzzle = new int[9, 9] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        /// <summary>
        /// Solution.
        /// </summary>
        private int[,] _solution = new int[9, 9] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        /// <summary>
        /// Prints this sudoku's solution in the console.
        /// </summary>
        public void PrintSolution() {
            StringBuilder sol = new StringBuilder("Solution: ");
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    sol.Append($"{_solution[row, col]}");

            UnityEngine.Debug.Log(sol.ToString());
        }

        /// <summary>
        /// Prints this sudoku's puzzle in the console, 0's represent empty cell/numbers.
        /// </summary>
        public void PrintPuzzle() {
            StringBuilder puz = new StringBuilder("  Puzzle: ");
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    puz.Append($"{_puzzle[row, col]}");

            UnityEngine.Debug.Log(puz.ToString());
        }
    }
}