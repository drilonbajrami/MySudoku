using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MySudoku
{
    /// <summary>
    /// Class for generating a sudoku with a solution and puzzle.
    /// </summary>
    public class SudokuGenerator : MonoBehaviour
    {
        [SerializeField] SolutionGenerator _solutionGenerator;
        [SerializeField] PuzzleGenerator _puzzleGenerator;

        // Add seeding for both generators
        // Add options if to use the same seed for both or random
        // Add options if different seeds used, then show the seed input

        /// <summary>
        /// Seed for the random generator.
        /// </summary>
        [SerializeField] private int _seed;

        /// <summary>
        /// Random number generator with seed.
        /// </summary>
        [SerializeField] RandomGenerator _random;


        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.< /returns>
        public Sudoku Generate(Difficulty difficulty)
        {
            if (_random == null) {
                Debug.LogWarning("Random generator does not exist! Please add one.");
                return null;
            }

            _random.SetSeed(_seed);
            int[,] solution = _solutionGenerator.Generate();
            int[,] puzzle = _puzzleGenerator.Generate(solution, difficulty);
            return puzzle == null ? null : new Sudoku(difficulty, solution, puzzle);
        }
    }
}