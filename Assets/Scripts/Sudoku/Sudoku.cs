using System.Text;

namespace MySudoku
{
    /// <summary>
    /// Sudoku class that stores a sudoku puzzle and its solution.
    /// </summary>
    public class Sudoku
    {
        /// <summary>
        /// The difficulty of this sudoku puzzle.
        /// </summary>
        public Difficulty Difficulty { get; private set; }

        /// <summary>
        /// Sudoku puzzle.
        /// </summary>
        public int[,] Puzzle { get; set; } = new int[9, 9] {
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
        /// Sudoku solution.
        /// </summary>
        public int[,] Solution { get; set; } = new int[9, 9] {
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
        public void PrintSolution()
        {
            StringBuilder sol = new StringBuilder("Solution: ");
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    sol.Append($"{Solution[row, col]}");

            UnityEngine.Debug.Log(sol.ToString());
        }

        /// <summary>
        /// Prints this sudoku's puzzle in the console, 0's represent empty cell/numbers.
        /// </summary>
        public string PrintPuzzle()
        {
            StringBuilder puz = new StringBuilder();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    puz.Append($"{Puzzle[row, col]}");

            //UnityEngine.Debug.Log(puz.ToString());
            return puz.ToString();
        }
    }
}