using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Naked Pairs technique is applicable when two cells within a box, row or column, have the same pair of candidates as the 
    /// only available candidates in them. This means that the candidates from this pair, can be removed from other cells within
    /// that box, row or column.
    /// More info on: https://www.sudokuoftheday.com/techniques/naked-pairs-triples.
    /// </summary>
    public class NakedPairs : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 750;

        /// <inheritdoc/>
        public int SubsequentUseCost => 500;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            Repetition[] cells = new Repetition[9] {
                new Repetition(0), new Repetition(1), new Repetition(2),
                new Repetition(3), new Repetition(4), new Repetition(5),
                new Repetition(6), new Repetition(7), new Repetition(8)
            };

            StringBuilder s = new("Naked Pair ");
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
                for (int boxCol = 0; boxCol < 9; boxCol += 3) {
                    // Clear are previous data from the candidate's repetitions.
                    for (int index = 0; index < 9; index++) cells[index].ClearAll();
                    int row = boxRow + boxCol / 3;
                    int col = boxRow / 3 + boxCol;

                    for (int k = 0; k < 9; k++) // Cell's index as [k].
                        for (int n = 0; n < 9; n++) { // Candidate's index as [i].
                            if (notes[(boxRow + k / 3) * 9 + boxCol + k % 3, n] && sudoku[boxRow + k / 3, boxCol + k % 3] == 0) cells[k].Box.Add(n);
                            if (notes[row * 9 + k, n] && sudoku[row, k] == 0) cells[k].Row.Add(n);
                            if (notes[k * 9 + col, n] && sudoku[k, col] == 0) cells[k].Col.Add(n);
                        }

                    bool applied = false;
                    for (int i = 0; i < 8; i++) {

                        // Check if possible in box.
                        if (cells[i].Box.Count == 2)
                            for (int j = i + 1; j < 9; j++)
                                if (HasPair(cells[i].Box, cells[j].Box)) {
                                    (int a, int b) pair = (cells[i].Box[0], cells[i].Box[1]);
                                    bool rowAvailable = i / 3 == j / 3;
                                    bool colAvailable = j - i == 3;
                                    // Go through each cell within box.
                                    for (int k = 0; k < 9; k++) {
                                        if (k == i || k == j) continue; // Skip over cells i and j.
                                        int kIndex = (boxRow + k / 3) * 9 + boxCol + k % 3;

                                        if (k != i && k != j) {
                                            if (!applied) applied = notes[kIndex, pair.a] || notes[kIndex, pair.b];
                                            notes[kIndex, pair.a] = false;
                                            notes[kIndex, pair.b] = false;
                                        }

                                        // Row || Col
                                        if (rowAvailable || colAvailable) {     // Cell[u] Coords       // Cell[v] Coords
                                            bool skipUVcells = rowAvailable ? ((k == boxCol + i % 3) || (k == boxCol + j % 3)) : ((k == boxRow + i / 3) || (k == boxRow + j / 3));
                                            if (skipUVcells) continue;
                                            int rcIndex = rowAvailable ? ((boxRow + i / 3) * 9 + k) : (k * 9 + boxCol + i % 3);
                                            if (!applied) applied = notes[rcIndex, pair.a] || notes[rcIndex, pair.b];
                                            notes[rcIndex, pair.a] = false;
                                            notes[rcIndex, pair.b] = false;
                                        }
                                    }

                                    if (applied) {
                                        s.Append($"[{pair.a + 1}, {pair.b + 1}] found on: \n");
                                        s.AppendLine($"Box ({boxRow}, {boxCol}) on cells ({boxRow + i / 3}, {boxCol + i % 3}) and ({boxRow + j / 3}, {boxCol + j % 3})");
                                        if (rowAvailable) s.AppendLine($"Row ({row}) as well.");
                                        if (colAvailable) s.AppendLine($"Col ({col}) as well.");
                                        Debug.Log(s.ToString());
                                        return true;
                                    } // => log entries && return true;
                                }

                        // Check if possible in row and column.
                        if (cells[i].Row.Count == 2 || cells[i].Col.Count == 2) {
                            for (int rank = 0; rank < 2; rank++)
                                for (int j = i + 1; j < 9; j++)
                                    if (rank == 0 ? HasPair(cells[i].Row, cells[j].Row) : HasPair(cells[i].Col, cells[j].Col)) {
                                        (int a, int b) pair = rank == 0 ? (cells[i].Row[0], cells[i].Row[1]) : (cells[i].Col[0], cells[i].Col[1]);
                                        for (int k = 0; k < 9; k++) {
                                            if (k == i || k == j) continue;
                                            int rcIndex = rank == 0 ? (row * 9 + k) : (k * 9 + col);
                                            if (!applied) applied = notes[rcIndex, pair.a] || notes[rcIndex, pair.b];
                                            notes[rcIndex, pair.a] = false;
                                            notes[rcIndex, pair.b] = false;
                                        }

                                        if (applied) {
                                            s.Append($"[{pair.a + 1}, {pair.b + 1}] found on: \n");
                                            s.AppendLine(i == 0 ? $"Row ({row}) on cells ({row}, {i}) and ({row}, {j})." :
                                                                  $"Col ({col}) on cells ({i}, {col}) and ({j}, {col}).");
                                            Debug.Log(s.ToString());
                                            return true;
                                        } // => log entries && return true;
                                    }
                        }
                    }
                }

            return false;
        }

        /// <summary>
        /// Checks if two sets are identical in size and elements, regardles of the order.
        /// </summary>
        /// <param name="l1">First set.</param>
        /// <param name="l2">Second set.</param>
        /// <returns>Whether these two sets are identical or not.</returns>
        private bool HasPair(List<int> l1, List<int> l2) => l1.Count == 2 && l1.Count == l2.Count && l1.All(l2.Contains);
    }
}