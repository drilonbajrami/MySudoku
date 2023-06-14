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
        public int TimesUsed { get; set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 100;

        /// <inheritdoc/>
        public int SubsequentUseCost => 100;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            //int runs = 0;
            //int totalRuns = 0;
            cost = 0;

            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    //runs++;
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;
                    
                    // Set single candidate flag to false.
                    int candidate = 0;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        //totalRuns++;
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
                        //double elapsedTime = sw.Elapsed.TotalMilliseconds;
                        //NakedSingleLogger.Instance.WriteData(runs, totalRuns, elapsedTime);
                        if (LogConsole) Debug.Log($"Naked Single: \n Cell ({row}, {col}) for {candidate}");
                        //Debug.Log($"Naked Single Technique applied in {sw.Elapsed.TotalMilliseconds} ms, {runs} runs | {totalRuns} total runs");
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