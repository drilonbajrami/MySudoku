using UnityEngine;
namespace MySudoku
{
    /// <summary>
    /// Naked Single or Single Candidate technique is applicable when an empty cell has only one candidate.
    /// More info on: https://www.sudokuoftheday.com/techniques/single-candidate.
    /// </summary>
    public class NakedSingle : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 100;

        /// <inheritdoc/>
        public int SubsequentUseCost => 100;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Set single candidate flag to false.
                    int candidate = 0;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        // If note is inactive then continue with the next note.
                        if (!notes[row * 9 + col, i]) continue;

                        // If there was a candidate already, then skip this cell.
                        if (candidate != 0) {
                            candidate = 0;
                            break;
                        }
                        else candidate = i + 1;
                    }

                    // If there was only one candidate then use its value.
                    if (candidate != 0) {
                        sudoku[row, col] = candidate;
                        notes.UpdateNotes(sudoku, (row, col), 0, candidate);
                        if (LogConsole) Debug.Log($"Naked Single: \n Cell ({row}, {col}) for {candidate}");
                        return true;
                    }
                }

            return false;
        }
    }
}