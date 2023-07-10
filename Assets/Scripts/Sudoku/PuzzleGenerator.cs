using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;

namespace MySudoku
{
    public class PuzzleGenerator : MonoBehaviour
    {
        /// <summary>
        /// The sudoku solver used for generating a sudoku puzzle.
        /// </summary>
        [SerializeField] SudokuSolver _solver;

        /// <summary>
        /// Seed for the random generator.
        /// </summary>
        [SerializeField] private int _seed;

        /// <summary>
        /// Random number generator with seed.
        /// </summary>
        [SerializeField] RandomGenerator _random;

        /// <summary>
        /// Threshold for a generation duration, if it exceeds this amount then we cancel it and start a new one.
        /// </summary>
        [Range(5, 30)]
        public double maxGenDurationInSeconds = 5;

        [Range(5, 30)]
        public int maxNumOfTriesToGenerate = 5;

        /// <summary>
        /// Generates a sudoku puzzle based on the given sudoku solution.
        /// </summary>
        /// <param name="solution">The sudoku solution.</param>
        /// <param name="difficulty">Difficulty of the generated sudoku puzzle.</param>
        /// <returns>A Sudoku puzzle if generated successfully, otherwise null.</returns>
        public int[,] Generate(int[,] solution, Difficulty difficulty)
        {
            _random.SetSeed(_seed);

            //ConsoleLog.Clear();
            Stopwatch stopwatch = new();
            Stopwatch recursiveTimer = new();

            // Create an array for the puzzle, notes and note's copy.
            int[,] puzzle = new int[9, 9];
            bool[,] notes = new bool[81, 9];
            bool[,] notesCopy = new bool[81, 9];

            // Flag for exiting while loop if puzzle has been generated.
            bool keepGenerating = true;

            // Set the difficulty score threshold/cap for this puzzle, based on the chosen difficulty.
            (int lower, int upper) difficultyRange = SudokuTechniques.DifficultyMap[difficulty];
            int difficultyScore = 0;
            int difficultyScoreCeiling = 0;

            int triesPerPuzzleGeneration = maxNumOfTriesToGenerate;
            double recursiveElapsedTime = 0;

            // Start trying to generate puzzle.
            while (keepGenerating) {
                stopwatch.Restart();
                recursiveElapsedTime = 0;

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
                Queue<(int, int)> ind = new(indexes);

                // Set the difficulty score threshold/cap for this puzzle, based on the chosen difficulty.
                difficultyScoreCeiling = _random.Next(difficultyRange.lower, difficultyRange.upper);
                difficultyScore = 0;

                while (difficultyScore < difficultyScoreCeiling && ind.Count > Sudoku.MIN_NUM_OF_CLUES) {
                    // Pick next index, store current value and update puzzle and notes
                    (int row, int col) index = ind.Dequeue();
                    int oldValue = puzzle[index.row, index.col];
                    puzzle[index.row, index.col] = 0;
                    notes.Update(puzzle, index, oldValue, 0);
                    Array.Copy(notes, notesCopy, notes.Length);

                    if (_solver.TechniqueSolve(puzzle, solution, notesCopy, logResult: false, out int cost) && cost < difficultyRange.upper)
                        difficultyScore = cost;
                    else {
                        ind.Enqueue(index);
                        puzzle[index.row, index.col] = oldValue;
                        notes.Update(puzzle, index, 0, oldValue);
                    }

                    if (stopwatch.Elapsed.TotalSeconds > maxGenDurationInSeconds) return null;
                }

                keepGenerating = difficultyScore < difficultyScoreCeiling || (difficultyScore < difficultyRange.lower || difficultyScore > difficultyRange.upper);

                if (!keepGenerating) {
                    recursiveTimer.Restart();
                    keepGenerating = !_solver.IsUnique(puzzle);
                    recursiveTimer.Stop();
                    stopwatch.Stop();
                    recursiveElapsedTime = recursiveTimer.Elapsed.TotalMilliseconds;
                }

                if (triesPerPuzzleGeneration < 0) {
                    //Debug.LogWarning($"----------FAILURE---------- | Ceiling: {difficultyScoreCeiling}");
                    return null;
                }

                triesPerPuzzleGeneration--;
            }

#if UNITY_EDITOR

            //Debug.Log($"----------SUCCESS---------- | Score: {difficultyScore} Ceiling: {difficultyScoreCeiling} - [{difficultyRange.lower}, {difficultyRange.upper}]");
            double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            //Debug.Log($"Recursive Time Log: Total Elapse vs Recursive Elapsed: {elapsedTime}, {recursiveElapsedTime}");
            //_recursiveSolverLogger.WriteData($"{difficulty},{difficultyScore},{elapsedTime},{recursiveElapsedTime}");
            //_difficulyScoreAndTechniquesLogger.WriteData($"{difficulty},{difficultyScore},{elapsedTime}," +
            //    $"{SudokuTechniques.Techniques[0].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[1].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[2].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[3].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[4].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[5].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[6].TimesUsed}," +
            //    $"{SudokuTechniques.Techniques[7].TimesUsed}");

            //ConsoleLog.Clear();
            //Debug.Log($"Selected difficulty: {difficulty} with score range [{difficultyRange.lower}, {difficultyRange.upper}] and score cap {difficultyCap}");
            //Debug.Log($"Difficulty Score: {difficultyScore}");
            //Debug.Log("Techniques: ");
            //for (int i = 0; i < SudokuTechniques.Techniques.Count; i++)
            //    Debug.Log($"    {(Technique)i} used {SudokuTechniques.Techniques[i].TimesUsed} times.");
            //GetPuzzle(puzzle).CopyToClipboard();
#endif

            return puzzle;
        }
    }
}