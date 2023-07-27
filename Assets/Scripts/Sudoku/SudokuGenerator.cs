using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Class for generating a sudoku solution and puzzle.
    /// </summary>
    public class SudokuGenerator : MonoBehaviour
    {
        [SerializeField] SolutionGenerator _solutionGenerator;
        [SerializeField] PuzzleGenerator _puzzleGenerator;

        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.< /returns>
        public Sudoku Generate(Difficulty difficulty)
        {
            int[,] solution = _solutionGenerator.Generate();
            int[,] puzzle = _puzzleGenerator.Generate(solution, difficulty);
            return puzzle == null ? null : new Sudoku(difficulty, solution, puzzle);
        }
    }
}