using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Hidden Single or Single Position technique is applicable when a candidate appears in one cell only
    /// within a n, row or column.
    /// More info on: https://www.sudokuoftheday.com/techniques/single-position.
    /// </summary>
    public class HiddenSingle : SudokuTechnique
    {
        protected override int FirstUseCost => 100;
        protected override int SubsequentUseCost => 100;

        public override bool Apply(int[,] sudoku, bool[,,] notes, out int cost)
        {
            cost = 0;

            // CELL GRID 9x9:
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {

                    // CELL:
                    if (sudoku[row, col] != 0) continue;  // Skip filled cells.

                    (int row, int col) boxCoord = new(row - (row % 3), col - (col % 3));

                    // CELL CANDIDATE:
                    for (int noteIndex = 0; noteIndex < 9; noteIndex++) {
                        if (!notes[row, col, noteIndex]) continue; // Skip inactive candidates.

                        // Track if the current candidate (candidateIndex) is a hidden single within its respective box, row or column.
                        bool isHiddenSingle;
                        bool hiddenInBox = true, hiddenInRow = true, hiddenInCol = true;

                        // First check if it is a hidden single within the box region, row or column neighbour cells of the current cell.
                        for (int neighbourIndex = 0; neighbourIndex < 9; neighbourIndex++) {

                            // Check if hidden in Box:
                            if (hiddenInBox) {
                                (int row, int col) n = new(boxCoord.row + (neighbourIndex / 3), boxCoord.col + (neighbourIndex % 3));
                                if ((row != n.row || col != n.col) && sudoku[n.row, n.col] == 0)
                                    hiddenInBox = !notes[n.row, n.col, noteIndex];
                            }

                            // Check if hidden in Row:
                            if (hiddenInRow && col != neighbourIndex && sudoku[row, neighbourIndex] == 0)
                                hiddenInRow = !notes[row, neighbourIndex, noteIndex];

                            // Check if hidden in Column:
                            if (hiddenInCol && row != neighbourIndex && sudoku[neighbourIndex, col] == 0)
                                hiddenInCol = !notes[neighbourIndex, col, noteIndex];

                            isHiddenSingle = hiddenInBox || hiddenInRow || hiddenInCol;

                            // Check if the current note is still a hidden single when processing all neighbor cells.
                            if (isHiddenSingle && neighbourIndex == 8) {
                                sudoku[row, col] = noteIndex + 1;
                                notes.Update(sudoku, (row, col), 0, noteIndex + 1);
                                cost = GetUsageCost();
                                if (LogConsole) LogTechnique(row, col, noteIndex, hiddenInBox, hiddenInRow, hiddenInCol);
                                return true;
                            }
                            else if (!isHiddenSingle) break; // Skip for current note if it is not a hidden single.
                        }
                    }
                }

            return false;
        }

        private void LogTechnique(int row, int col, int candidateIndex, bool hiddenInBox, bool hiddenInRow, bool hiddenInCol)
            => Debug.Log($"HIDDEN SINGLE: Cell[{row}, {col}] for {candidateIndex + 1}: " +
                         $"{(hiddenInBox ? $"Box[{row - (row % 3)}, {col - (col % 3)}]" : "")} " +
                         $"{(hiddenInRow ? $"Row[{row}]" : "")} " +
                         $"{(hiddenInCol ? $"Col[{col}]" : "")}");


        //public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        //{
        //    cost = 0;

        //    for (int row = 0; row < 9; row++)
        //        for (int col = 0; col < 9; col++)
        //            if (sudoku[row, col] == 0) {
        //                if (TryApplyHiddenSingle(sudoku, notes, row, col, out cost)) {
        //                    return true;
        //                }
        //            }

        //    return false;
        //}

        //private bool TryApplyHiddenSingle(int[,] sudoku, bool[,] notes, int row, int col, out int cost)
        //{
        //    cost = 0;
        //    (int row, int col) boxCoord = new(row - (row % 3), col - (col % 3));

        //    for (int noteIndex = 0; noteIndex < 9; noteIndex++) {
        //        if (!notes[row * 9 + col, noteIndex]) continue;

        //        bool isHiddenSingle = IsHiddenSingle(sudoku, notes, row, col, boxCoord, noteIndex);

        //        if (isHiddenSingle) {
        //            sudoku[row, col] = noteIndex + 1;
        //            notes.Update(sudoku, (row, col), 0, noteIndex + 1);
        //            cost = GetUsageCost();
        //            LogTechnique(row, col, noteIndex);
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private bool IsHiddenSingle(int[,] sudoku, bool[,] notes, int row, int col, (int row, int col) boxCoord, int noteIndex)
        //{
        //    bool hiddenInBox = CheckHiddenInBox(sudoku, notes, row, col, boxCoord, noteIndex);
        //    bool hiddenInRow = CheckHiddenInRow(sudoku, notes, row, col, noteIndex);
        //    bool hiddenInCol = CheckHiddenInCol(sudoku, notes, row, col, noteIndex);

        //    return hiddenInBox || hiddenInRow || hiddenInCol;
        //}

        //private bool CheckHiddenInBox(int[,] sudoku, bool[,] notes, int row, int col, (int row, int col) boxCoord, int noteIndex)
        //{
        //    bool hiddenInBox = true;

        //    for (int neighbourIndex = 0; neighbourIndex < 9; neighbourIndex++) {
        //        (int neighbourRow, int neighbourCol) = GetNeighbourCell(boxCoord, neighbourIndex);

        //        if ((row != neighbourRow || col != neighbourCol) && sudoku[neighbourRow, neighbourCol] == 0) {
        //            hiddenInBox = !notes[neighbourRow * 9 + neighbourCol, noteIndex];
        //        }

        //        if (!hiddenInBox)
        //            break;
        //    }

        //    return hiddenInBox;
        //}

        //private bool CheckHiddenInRow(int[,] sudoku, bool[,] notes, int row, int col, int noteIndex)
        //{
        //    bool hiddenInRow = true;

        //    for (int neighbourIndex = 0; neighbourIndex < 9; neighbourIndex++) {
        //        if (col != neighbourIndex && sudoku[row, neighbourIndex] == 0) {
        //            hiddenInRow = !notes[row * 9 + neighbourIndex, noteIndex];
        //        }

        //        if (!hiddenInRow)
        //            break;
        //    }

        //    return hiddenInRow;
        //}

        //private bool CheckHiddenInCol(int[,] sudoku, bool[,] notes, int row, int col, int noteIndex)
        //{
        //    bool hiddenInCol = true;

        //    for (int neighbourIndex = 0; neighbourIndex < 9; neighbourIndex++) {
        //        if (row != neighbourIndex && sudoku[neighbourIndex, col] == 0) {
        //            hiddenInCol = !notes[neighbourIndex * 9 + col, noteIndex];
        //        }

        //        if (!hiddenInCol)
        //            break;
        //    }

        //    return hiddenInCol;
        //}

        //private void LogTechnique(int row, int col, int candidateIndex)
        //    => Debug.Log($"HIDDEN SINGLE: Cell[{row}, {col}] for {candidateIndex + 1}");

        //private (int row, int col) GetNeighbourCell((int row, int col) boxCoord, int neighbourIndex)
        //    => (boxCoord.row + (neighbourIndex / 3), boxCoord.col + (neighbourIndex % 3));
    }
}