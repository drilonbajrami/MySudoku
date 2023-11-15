using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MySudoku
{
    [RequireComponent(typeof(RandomGenerator))]
    public class PuzzleGenerator : MonoBehaviour
    {
        /// <summary>
        /// Seed for the random randomGenerator.
        /// </summary>
        [SerializeField] int _seed;

        /// <summary>
        /// Random number randomGenerator with seed.
        /// </summary>
        [SerializeField] RandomGenerator _random;

        /// <summary>
        /// The sudoku solver used for generating a sudoku puzzle.
        /// </summary>
        [SerializeField] SudokuSolver _solver;

        /// <summary>
        /// Threshold for a generation duration, if it exceeds this amount then we cancel it and start a new one.
        /// </summary>
        [Range(5, 30)]
        public double maxGenDurationInSeconds = 5;

        /// <summary>
        /// Generates a sudoku puzzle based on the given sudoku solution.
        /// </summary>
        /// <param name="solution">The sudoku solution.</param>
        /// <param name="difficulty">Difficulty of the generated sudoku puzzle.</param>
        /// <returns>A Sudoku puzzle if generated successfully, otherwise null.</returns>
        public int[,] Generate(int[,] solution, Difficulty difficulty)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Create an array for the puzzle, notes and note's copy.
            int[,] puzzle = new int[9, 9];
            bool[,] notes = new bool[81, 9];
            bool[,] notesCopy = new bool[81, 9];

            // Set the difficulty score threshold/cap for this puzzle, based on the chosen difficulty.
            (int lower, int upper) difficultyRange = SudokuTechniques.DifficultyMap[difficulty];
            int difficultyScore;
            int difficultyScoreCeiling;

            // Flag for exiting while loop if puzzle has been generated.
            bool keepGenerating = true;

            // Start trying to generate puzzle.
            while (keepGenerating) {
                // Cache all sudoku grid cells by their indexes and clear the notes.
                // And copy the solution to the puzzle array.
                List<(int row, int col)> indexes = new();
                for (int row = 0; row < 9; row++)
                    for (int col = 0; col < 9; col++) {
                        puzzle[row, col] = solution[row, col];
                        indexes.Add((row, col));
                        for (int i = 0; i < 9; i++)
                            notes[row * 9 + col, i] = notesCopy[row * 9 + col, i] = false;
                    }

                // Shuffle the indexes and add them into a queue.
                indexes.Shuffle(_random);
                Queue<(int, int)> indexQueue = new(indexes);

                // Set the difficulty score threshold/cap for this puzzle, based on the chosen difficulty.
                difficultyScoreCeiling = RandomNumberIn50s(difficultyRange.lower, difficultyRange.upper);
                difficultyScore = 0;

                while (difficultyScore < difficultyScoreCeiling && indexQueue.Count > 0) {
                    // Pick next index, store current value and update puzzle and notes
                    (int row, int col) index = indexQueue.Dequeue();
                    int oldValue = puzzle[index.row, index.col];
                    puzzle[index.row, index.col] = 0;
                    notes.Update(puzzle, index, oldValue, 0);

                    // Copy the notes array to a separate array (notesCopy) to pass it into the TechniqueSolver
                    // without modifying the original notes array.
                    Array.Copy(notes, notesCopy, notes.Length);

                    if (_solver.TechniqueSolve(puzzle, solution, notesCopy, logResult: false, out int cost) && cost < difficultyRange.upper)
                        difficultyScore = cost;
                    else {
                        puzzle[index.row, index.col] = oldValue;
                        notes.Update(puzzle, index, 0, oldValue);
                    }
                }

                keepGenerating = difficultyScore < difficultyScoreCeiling ||
                                 difficultyScore < difficultyRange.lower ||
                                 difficultyScore > difficultyRange.upper ||
                                 !_solver.IsUnique(puzzle);

                if (stopwatch.Elapsed.TotalSeconds > maxGenDurationInSeconds) {
                    Debug.Log($"Generation took longer than the set time limit of {maxGenDurationInSeconds} seconds.");
                    return null;
                }
            }

            //GetPuzzle(puzzle).CopyToClipboard();
            return puzzle;
        }

        /// <summary>
        /// Returns a random number divisible by 50, between the lower and upper given ranges.
        /// </summary>
        private int RandomNumberIn50s(int lower, int upper)
        {
            // Adjust the lower and upper bounds to the nearest multiple of 50
            int lowerAdjusted = lower - (lower % 50) + 50;
            int upperAdjusted = upper - (upper % 50);

            // Calculate the number of valid options within the adjusted range
            int optionsCount = (upperAdjusted - lowerAdjusted) / 50 + 1;

            // Generate a random index within the number of options
            int index = _random.Next(optionsCount);

            // Calculate the final random number using the index and adjusted lower bound
            int number = lowerAdjusted + index * 50;

            return number;
        }

        public void SetRandomGenerator(RandomGenerator randomGenerator) => _random = randomGenerator;

        /// <summary>
        /// Sets the seed and uses own random generator for the puzzle generator.
        /// </summary>
        public void SetSeed(int seed)
        {
            _seed = seed;
            _random = GetComponent<RandomGenerator>();
            _random.SetSeed(_seed);
        }
    }
}