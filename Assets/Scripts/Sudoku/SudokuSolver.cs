using System;
using System.Collections;
using System.Collections.Generic;
using MySudoku;
using UnityEngine;

public class SudokuSolver : MonoBehaviour
{
    /// <summary>
    /// Tries to solve a sudoku puzzle with currently available sudoku techniques.
    /// </summary>
    /// <param name="puzzleTemplate">The sudoku puzzle.</param>
    /// <param name="solution">The sudoku solution.</param>
    /// <param name="notesCopy">The notes for the sudoku puzzle.</param>
    /// <returns>Whether this sudoku puzzle can be solved with currently available techniques.</returns>
    public bool TechniqueSolve(int[,] puzzleTemplate, int[,] solution, bool[,] notesCopy, bool logResult, out int difficultyCost)
    {
        int[,] puzzle = new int[9, 9];
        Array.Copy(puzzleTemplate, puzzle, puzzleTemplate.Length);
        SudokuTechniques.ResetTechniqueUsageCount();

        bool solving = true;
        difficultyCost = 0;
        int cost;

        while (solving) {
            cost = puzzle.ApplyTechniques(notesCopy);
            solving = cost != 0;
            difficultyCost += solving ? cost : 0;
            if (puzzle.IsIdenticalTo(solution)) {
                if (logResult) Debug.Log($"Solved with difficulty cost of {difficultyCost}.");
                return true;
            };
        }

        return false;
    }

    /// <summary>
    /// Checks if a sudoku puzzle has a unique solution or not.
    /// </summary>
    /// <param name="puzzle">Sudoku puzzle.</param>
    /// <returns>Whether this sudoku is unique or not.</returns>
    public bool IsUnique(int[,] puzzle) => BacktrackSolve(puzzle, true) == 1;

    /// <summary>
    /// Returns the number of available solutions for a sudoku puzzle.
    /// </summary>
    /// <param name="puzzle">Sudoku puzzle to solve.</param>
    /// <param name="checkForUniqueness">Check if given puzzle has a unique solution or not.</param>
    /// <param name="row">Current row.</param>
    /// <param name="col">Current column.</param>
    /// <param name="solutionCount">Number of solutions.</param>
    /// <returns>Number of solutions for this sudoku puzzle if not checking for uniqueness.<br/>
    /// If yes, then it will return these options:<br/>
    /// <b><i>0</i></b> -> no solutions found.<br/>
    /// <b><i>1</i></b> -> has a unique solution.<br/>
    /// <b><i>2</i></b> -> has more than one solution, it is not unique.<br/>
    /// <b><i>Note: When checking for uniqueness, the backtrack solver will stop checking for other possible solutions <br/>
    /// if more than one solution is found.</i></b></returns>
    public int BacktrackSolve(int[,] puzzle, bool checkForUniqueness, int row = 0, int col = 0, int solutionCount = 0)
    {
        if (checkForUniqueness && solutionCount > 1) return solutionCount;

        if (col == 9) {
            col = 0;
            if (++row == 9) return ++solutionCount;
        }

        // Skip cells that are not empty.
        if (puzzle[row, col] != 0) return BacktrackSolve(puzzle, checkForUniqueness, row, col + 1, solutionCount);

        for (int val = 1; val <= 9; ++val)
            if (puzzle.CanUseNumber(row, col, val)) {
                puzzle[row, col] = val;
                // Add additional solutions if possible.
                solutionCount = BacktrackSolve(puzzle, checkForUniqueness, row, col + 1, solutionCount);
            }

        // Reset on backtrack.
        puzzle[row, col] = 0;
        return solutionCount;
    }
}