using System.Text;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Multiple Lines technique is applicable when a candidate appears on two same rows or columns on two boxes and, 
    /// meaning that that candidate can be removed from those two rows or column from the third box.
    /// More info on: https://www.sudokuoftheday.com/techniques/multiple-lines.
    /// </summary>
    public class MultipleLines : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; private set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 600;

        /// <inheritdoc/>
        public int SubsequentUseCost => 300;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public void ResetUseCount() => TimesUsed = 0;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            StringBuilder s = new();
            // Process only the diagonal boxes (1, 5, 9)
            for (int box = 0; box < 9; box += 3) {
                // Check for each note.
                for (int i = 0; i < 9; i++) {
                    // Keep track of rows where this note is present in boxes.
                    // boxPerRow[box index, row index].
                    bool[,] boxPerRow = new bool[3, 3];

                    // Keep track of columns where this note is present in boxes.
                    // boxPerCol[box index, column index].
                    bool[,] boxPerCol = new bool[3, 3];

                    // d -> row or column index.
                    // k -> index of a cell in a row or column.
                    for (int d = 0; d < 3; d++) {
                        for (int k = 0; k < 9; k++) {
                            if (sudoku[d + box, k] == 0) boxPerRow[k / 3, d] = boxPerRow[k / 3, d] || notes[(d + box) * 9 + k, i];
                            if (sudoku[k, d + box] == 0) boxPerCol[k / 3, d] = boxPerCol[k / 3, d] || notes[k * 9 + d + box, i];
                        }
                    }

                    if (LogConsole) s = new("MULTIPLE LINES: \n");

                    // Check if this technique is applicable on the row direction of the currently processed boxes/regions.
                    if (IsMultipleLinesApplicable(boxPerRow, out int selectedBoxR, out int selectedRow)) {
                        if (LogConsole) s.AppendLine($"BOX {box + selectedBoxR} - Keep-ROW {selectedRow + box} for note {i + 1} -> removing from: ");
                        for (int r = 0; r < 3; r++) {
                            if (r != selectedRow)
                                for (int c = 0; c < 3; c++) {
                                    notes[(box + r) * 9 + selectedBoxR * 3 + c, i] = false;
                                    if (LogConsole) s.Append($"({box + r}, {selectedBoxR * 3 + c}), ");
                                }
                        }
                        if (LogConsole) Debug.Log(s.ToString());
                        TimesUsed++;
                        cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                        return true;
                    }

                    // Check if this technique is applicable on the column direction of the currently processed boxes/regions.
                    if (IsMultipleLinesApplicable(boxPerCol, out int selectedBoxC, out int selectedCol)) {
                        if (LogConsole) s.Append($"BOX {box + selectedBoxC} - Keep-COL {selectedCol + box} for note {i + 1} -> removing from: ");
                        for (int c = 0; c < 3; c++) {
                            if (c != selectedCol)
                                for (int r = 0; r < 3; r++) {
                                    notes[(selectedBoxC * 3 + r) * 9 + box + c, i] = false;
                                    if (LogConsole) s.Append($"({selectedBoxC * 3 + r}, {box + c}), ");
                                }
                        }
                        if (LogConsole) Debug.Log(s.ToString());
                        TimesUsed++;
                        cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the <see cref="ApplyTechnique(int[,], bool[,])"/> can be applied to the given boxes, on a row or column direction.
        /// </summary>
        /// <param name="boxs">The boxes/regions to check for.</param>
        /// <param name="selectedBox">The box where there should be removal of a note.</param>
        /// <param name="selectedRowOrCol">The row/column to keep, by removing the notes on the other two remaining rows/columns.</param>
        /// <returns>Whether the technique is applicable, and the box on which to edit the notes and<br/> 
        /// the row or column of the box on which to keep the notes active.</returns>
        private bool IsMultipleLinesApplicable(bool[,] boxs, out int selectedBox, out int selectedRowOrCol)
        {
            // 'p' and 'q' are for caching the indexes of the boxes that should not be changed,
            // in other words they are the same in terms of a note occurrence on the same rows/columns of these boxes.
            int p = -1;
            int q = -1;

            // 'toBox' is for caching the index of the box that needs to be changed/edited.
            selectedBox = -1;

            // It is used to cache the index of the row/column of the seleced box,
            // based on this the note occurrences in the two remaining rows/columns will be removed.
            selectedRowOrCol = -1;

            // Add up the number of note occurrences in rows/columns for each box. 
            int[] boxSums = new int[3];
            for (int i = 0; i < 3; i++)
                boxSums[i] = (boxs[i, 0] ? 1 : 0) + (boxs[i, 1] ? 1 : 0) + (boxs[i, 2] ? 1 : 0);

            // If at least each box contains the same note in more than one row/column,
            // and the total number of times a note appears on rows/columns in all three boxes should be at between 6 or 7 times.
            // The reason that the sum of all occurrences is checked only for 7,
            // is that the first condition of each box having a number of occurrences higher than 2 will dictate if it is equal to or greater than 6. 
            // The for loop checks the possiblites of equality between boxes in this order by their indexes ->
            // box[p] == box[q] - where (p,q) => (0,1), (1, 0), (2,0).
            if (boxSums[0] >= 2 && boxSums[1] >= 2 && boxSums[2] >= 2 && (boxSums[0] + boxSums[1] + boxSums[2]) <= 7) {
                for (int i = 0; i < 3; i++) {
                    p = i % 3;
                    q = (i + 1) % 3;
                    if (boxs[p, 0] == boxs[q, 0] && boxs[p, 1] == boxs[q, 1] && boxs[p, 2] == boxs[q, 2])
                        selectedBox = (i + 2) % 3;
                }

                // If no box has been assigned as the box to edit then skip everything. It is not possible to apply this technique anymore.
                if (selectedBox == -1) return false;

                // Once the index of a box to change is assigned, then check which row/column should not be edited in that particular box.
                for (int i = 0; i < 3; i++)
                    if (boxs[selectedBox, i] == (!boxs[p, i] || !boxs[q, i]) == true)
                        selectedRowOrCol = i;

                return selectedRowOrCol != -1;
            }
            else return false;
        }
    }
}