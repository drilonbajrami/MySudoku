using System.Text;

namespace MySudoku
{
    /// <summary>
    /// Sudoku class that stores a sudoku puzzle and its solution.
    /// </summary>
    public class Sudoku
    {
        public const int MIN_NUM_OF_CLUES = 17;

        public Difficulty Difficulty { get; private set; }

        public int[,] Solution { get; private set; }

        public int[,] Puzzle { get; private set; }

        public Sudoku(Difficulty difficulty, int[,] solution, int[,] puzzle)
        {
            Difficulty = difficulty;
            Solution = solution;
            Puzzle = puzzle;
        }

        public Sudoku()
        {
            Difficulty = Difficulty.Beginner;
            Solution = new int[9, 9];
            Puzzle = new int[9, 9];
        }

        /// <summary>
        /// Prints this sudoku's solution in the console.
        /// </summary>
        public string PrintSolution()
        {
            StringBuilder solution = new("Solution: ");
            solution.Append(ConvertArrayToString(Solution));
            //UnityEngine.Debug.Log(solution.ToString());
            return solution.ToString();
        }

        /// <summary>
        /// Prints this sudoku's puzzle in the console, 0's represent empty cell/numbers.
        /// </summary>
        public string PrintPuzzle()
        {
            StringBuilder puzzle = new("Puzzle: ");
            puzzle.Append(ConvertArrayToString(Puzzle));
            //UnityEngine.Debug.Log(puz.ToString());
            return puzzle.ToString();
        }

        private string ConvertArrayToString(int[,] array)
        {
            StringBuilder stringBuilder = new();

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    stringBuilder.Append(array[row, col]);
                
            return stringBuilder.ToString();
        }
    }
}