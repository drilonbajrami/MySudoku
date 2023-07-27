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
        public bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    if (sudoku[row, col] != 0) continue;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        if (!notes[row * 9 + col, i]) continue;

                        // Track if the current candidate (i) is a hidden single within its respective box, row and .
                        bool isHiddenSingle;
                        bool hiddenInBox = true, hiddenInRow = true, hiddenInCol = true;

                        // First check if it is a hidden single within the box/region, row or column of the current cell.
                        for (int j = 0; j < 9; j++) {
                            int boxRow = row - (row % 3) + (j / 3);
                            int boxCol = col - (col % 3) + (j % 3);

                            // Box, row and column
                            if (hiddenInBox && (row != boxRow || col != boxCol) && sudoku[boxRow, boxCol] == 0) hiddenInBox = !notes[boxRow * 9 + boxCol, i];
                            if (hiddenInRow && col != j && sudoku[row, j] == 0) hiddenInRow = !notes[row * 9 + j, i];
                            if (hiddenInCol && row != j && sudoku[j, col] == 0) hiddenInCol = !notes[j * 9 + col, i];

                            isHiddenSingle = hiddenInBox || hiddenInRow || hiddenInCol;

                            // Check if the current note is still a hidden single when processing all neighbor cells.
                            if (isHiddenSingle && j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.Update(sudoku, (row, col), 0, i + 1);
                                TimesUsed++;
                                cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                if (LogConsole) Debug.Log($"HIDDEN SINGLE: Cell[{row}, {col}] for {i + 1}: " +
                                    $"{(hiddenInBox ? $"Box[{row - (row % 3)}, {col - (col % 3)}]" : "")} " +
                                    $"{(hiddenInRow ? $"Row[{row}]" : "")} " +
                                    $"{(hiddenInCol ? $"Col[{col}]" : "")}");
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