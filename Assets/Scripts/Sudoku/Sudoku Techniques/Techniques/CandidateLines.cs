using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Candidate Lines or Pointing Pairs technique is applicable when a candidate appears on a row or column only, within a box.
    /// This means that the candidate can be removed from other cells on the same row or column, on other boxes.
    /// More info on: https://www.sudokuoftheday.com/techniques/candidate-lines.
    /// </summary>
    public class CandidateLines : SudokuTechnique
    {
        protected override int FirstUseCost => 350;
        protected override int SubsequentUseCost => 200;

        /// <inheritdoc/>
        public override bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            cost = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Process each active note of that cell.
                    for (int i = 0; i < 9; i++) {
                        // If note is not active then skip to next one.
                        if (!notes[row * 9 + col, i]) continue;

                        bool rowAvailable = false;
                        bool colAvailable = false;

                        // Check if the current note appears only within a row or column within this box.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - (row % 3) + (j / 3);
                            int nCol = col - (col % 3) + (j % 3);
                            bool isRowNeighbor = row == nRow && col != nCol;
                            bool isColNeighbor = row != nRow && col == nCol;

                            // If the cell being process is the current selected cell then skip.
                            if (!((row != nRow || col != nCol) && sudoku[nRow, nCol] == 0)) continue;

                            // Can't be applied if this note (i) appears on a different row and column within this box/region.
                            if (!isRowNeighbor && !isColNeighbor && notes[nRow * 9 + nCol, i]) {
                                rowAvailable = colAvailable = true;
                                break;
                            }

                            rowAvailable = !rowAvailable && isRowNeighbor ? notes[nRow * 9 + nCol, i] : rowAvailable;
                            colAvailable = !colAvailable && isColNeighbor ? notes[nRow * 9 + nCol, i] : colAvailable;
                        }

                        bool techniqueIsApplied = false;

                        // If candidate has appared more than once within a row or column in this box,
                        // then check for neighbor cells on the same row or column in other boxes.
                        if (rowAvailable == !colAvailable) {
                            List<(int row, int col)> cand = new();
                            int currentBox = rowAvailable ? col - col % 3 : row - row % 3;
                            for (int j = 0; j < 9; j++) {
                                int r = rowAvailable ? row : j;
                                int c = rowAvailable ? j : col;
                                if ((j < currentBox || j > currentBox + 2) && sudoku[r, c] == 0 && notes[r * 9 + c, i]) {
                                    notes[r * 9 + c, i] = false;
                                    techniqueIsApplied = true;
                                    cand.Add((r, c));
                                }
                            }

                            if (techniqueIsApplied) {
                                cost = GetUsageCost();
                                if (LogConsole) {
                                    StringBuilder s = new("CANDIDATE LINES: \n");
                                    s.AppendLine(rowAvailable ? $"Candidate [{i + 1}] on Row({row}) removed from cells: " :
                                                                $"Candidate [{i + 1}] on Col({col}) removed from cells: ");
                                    for (int x = 0; x < cand.Count; x++)
                                        s.Append($"({cand[x].row}, {cand[x].col}){(x == cand.Count - 1 ? "." : ", ")}");
                                    Debug.Log(s.ToString());
                                }
                                return true;
                            }
                        }
                    }
                }

            return false;
        }
    }
}