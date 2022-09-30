using System.Collections.Generic;
using System.Diagnostics;
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

        public List<int> GetSolution() {
            List<int> solution = new List<int>();
            StringBuilder sol = new StringBuilder();
            sol.Append("Solution: ");

            for (int j = 0; j < 9; j++)
                for (int i = 0; i < 9; i++) {
                    solution.Add(_solution[j, i]);
                    sol.Append($"{_solution[j, i]}");
                }
            UnityEngine.Debug.Log(sol.ToString());
            return solution;
        }

        public List<int> GetPuzzle() {
            List<int> puzzle = new List<int>();
            StringBuilder puz = new StringBuilder();
            puz.Append("  Puzzle: ");

            for (int j = 0; j < 9; j++)
                for (int i = 0; i < 9; i++) {
                    puzzle.Add(_puzzle[j, i]);
                    puz.Append($"{_puzzle[j, i]}");
                }

            UnityEngine.Debug.Log(puz.ToString());
            return puzzle;
        }
    }
}