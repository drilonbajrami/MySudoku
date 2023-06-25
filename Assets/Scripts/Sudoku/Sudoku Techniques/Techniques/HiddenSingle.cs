using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MySudoku
{
    /// <summary>
    /// Hidden Single or Single Position technique is applicable when a candidate appears in one cell only
    /// within a box, row or column.
    /// More info on: https://www.sudokuoftheday.com/techniques/single-position.
    /// </summary>
    public class HiddenSingle : ISudokuTechnique
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
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            cost = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        if (!notes[row * 9 + col, i]) continue;

                        // Track if the current candidate (i) is a hidden single.
                        bool isHiddenSingle;
                        bool isHiddenInBox = true;
                        bool isHiddenInRow = true;
                        bool isHiddenInCol = true;

                        // First check if it is a hidden single within the box/region, row or column of the current cell.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - (row % 3) + (j / 3);
                            int nCol = col - (col % 3) + (j % 3);

                            // Box, row and column
                            if (isHiddenInBox && (row != nRow || col != nCol) && sudoku[nRow, nCol] == 0) isHiddenInBox = !notes[nRow * 9 + nCol, i];
                            if (isHiddenInRow && col != j && sudoku[row, j] == 0) isHiddenInRow = !notes[row * 9 + j, i];
                            if (isHiddenInCol && row != j && sudoku[j, col] == 0) isHiddenInCol = !notes[j * 9 + col, i];

                            isHiddenSingle = isHiddenInBox || isHiddenInRow || isHiddenInCol;

                            // Check if the current note is still a hidden single when processing all neighbor cells.
                            if (isHiddenSingle && j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.Update(sudoku, (row, col), 0, i + 1);
                                TimesUsed++;
                                cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                if (LogConsole) Debug.Log($"HIDDEN SINGLE: Cell ({row}, {col}) for {i + 1}: {(isHiddenInBox ? "B" : "")} {(isHiddenInRow ? "R" : "")} {(isHiddenInCol ? "C" : "")}");
                                //Debug.Log($"Hidden Single Technique applied in {sw.Elapsed.TotalMilliseconds} ms");
                                return true;
                            }
                            else if (!isHiddenSingle) break; // Skip for current note if it is not a hidden single.
                        }
                    }
                }

            return false;
        }
    }
}