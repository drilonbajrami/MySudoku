using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Naked Single or Single Candidate technique is applicable when an empty cell has only one candidate.
    /// More info on: https://www.sudokuoftheday.com/techniques/single-candidate.
    /// </summary>
    public class NakedSingle : SudokuTechnique
    {
        protected override int FirstUseCost => 100;
        protected override int SubsequentUseCost => 100;

        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;

            // CELL GRID 9x9
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {

                    //CELL:
                    if (sudoku[row, col] != 0) continue; // Skip filled cells.

                    int candidate = FindSingleCandidate(notes, row * 9 + col);

                    // If a single candidate was found then apply it (0 - means no candidate found).
                    if (candidate != 0) {
                        sudoku[row, col] = candidate;
                        notes.Update(sudoku, (row, col), 0, candidate);
                        cost = GetUsageCost();
                        if (LogConsole) LogTechnique(row, col, candidate);
                        return true;
                    }
                }

            return false;
        }

        /// <returns>Returns 0 if there is no single candidate, otherwise returns the candidate itself.</returns>
        private int FindSingleCandidate(bool[,] notes, int cellIndex)
        {
            int candidate = 0;

            // Check each note of the cell.
            for (int noteIndex = 0; noteIndex < 9; noteIndex++) {
                if (!notes[cellIndex, noteIndex]) continue; // Skips inactive candidates.
                if (candidate != 0) return 0; // Returns 0 since there is more than one active candidate.
                candidate = noteIndex + 1;
            }

            return candidate;
        }

        private void LogTechnique(int row, int col, int candidate) 
            => Debug.Log($"NAKED SINGLE: Cell[{row}, {col}] for {candidate}");
    }
}