using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MySudoku
{
    /// <summary>
    /// Naked Single or Single Candidate technique is applicable when an empty cell has only one candidate.
    /// More info on: https://www.sudokuoftheday.com/techniques/single-candidate.
    /// </summary>
    public class NakedSingle : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; private set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 100;

        /// <inheritdoc/>
        public int SubsequentUseCost => 100;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public void ResetUseCount() => TimesUsed = 0;

        /// <inheritdoc/>
        public bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    if (sudoku[row, col] != 0) continue;
                    int candidate = 0;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        if (!notes[row * 9 + col, i]) continue;
                        // If there was a candidate already, then skip this cell and its notes.
                        if (candidate != 0) {
                            candidate = 0;
                            break;
                        }
                        else candidate = i + 1;
                    }

                    // If there was only one candidate then use its value.
                    if (candidate != 0) {
                        if (LogConsole) Debug.Log($"NAKED SINGLE: Cell[{row}, {col}] for {candidate}");            
                        sudoku[row, col] = candidate;
                        notes.Update(sudoku, (row, col), 0, candidate);
                        TimesUsed++;
                        cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                        return true;
                    }
                }
            
            return false;
        }
    }
}