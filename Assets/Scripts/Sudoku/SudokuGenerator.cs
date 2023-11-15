using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Class for generating a sudoku solution and puzzle.
    /// </summary>
    [RequireComponent(typeof(RandomGenerator))]
    public class SudokuGenerator : MonoBehaviour
    {
        [SerializeField] SolutionGenerator _solutionGenerator;
        [SerializeField] PuzzleGenerator _puzzleGenerator;
        [SerializeField] RandomGenerator _randomGenerator;

        [SerializeField] private bool _useOwnRandomGeneartor;
        [SerializeField] private int _seed;

        [SerializeField] private int _solutionGeneratorSeed;

        [Tooltip("The puzzle generator with a set seed, does not always return the same results if the solution random generator seed is set to 0.")]
        [SerializeField] private int _puzzleGeneratorSeed;

        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.< /returns>
        public Sudoku Generate(Difficulty difficulty)
        {
            if(_useOwnRandomGeneartor) {
                _solutionGenerator.SetRandomGenerator(_randomGenerator);
                _puzzleGenerator.SetRandomGenerator(_randomGenerator);
                _randomGenerator.SetSeed(_seed);
            }
            else {
                _solutionGenerator.SetSeed(_solutionGeneratorSeed);
                _puzzleGenerator.SetSeed(_puzzleGeneratorSeed);
            }

            int[,] solution = _solutionGenerator.Generate();
            int[,] puzzle = _puzzleGenerator.Generate(solution, difficulty);
            return puzzle == null ? null : new Sudoku(difficulty, solution, puzzle);
        }
    }
}