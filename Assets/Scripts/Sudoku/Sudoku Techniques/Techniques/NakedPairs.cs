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
        public int TimesUsed { get; private set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 750;

        /// <inheritdoc/>
        public int SubsequentUseCost => 500;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public void ResetUseCount() => TimesUsed = 0;

        /// <inheritdoc/>
        public bool Apply(int[,] sudoku, bool[,] notes, out int cost)
        {
            int numberOfSets = 3;
            Repetition[] cells = new Repetition[9] {
                new Repetition(0, numberOfSets), new Repetition(1, numberOfSets), new Repetition(2, numberOfSets),
                new Repetition(3, numberOfSets), new Repetition(4, numberOfSets), new Repetition(5, numberOfSets),
                new Repetition(6, numberOfSets), new Repetition(7, numberOfSets), new Repetition(8, numberOfSets)
            };

            cost = 0;
            StringBuilder s = new("Naked Pair ");
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
                for (int boxCol = 0; boxCol < 9; boxCol += 3) {
                    // Clear are previous data from the candidate's repetitions.
                    for (int index = 0; index < 9; index++) cells[index].ClearAll();
                    int row = boxRow + boxCol / 3;
                    int col = boxRow / 3 + boxCol;

                    for (int k = 0; k < 9; k++) // Cell's index as [k].
                        for (int n = 0; n < 9; n++) { // Candidate's index as [i].
                            if (notes[(boxRow + k / 3) * 9 + boxCol + k % 3, n] && sudoku[boxRow + k / 3, boxCol + k % 3] == 0) cells[k].Repetitions[0].Add(n);
                            if (notes[row * 9 + k, n] && sudoku[row, k] == 0) cells[k].Repetitions[1].Add(n);
                            if (notes[k * 9 + col, n] && sudoku[k, col] == 0) cells[k].Repetitions[2].Add(n);
                        }

                    (int a, int b) nakedPair = (-1, -1);
                    bool applied = false;

                    for (int i = 0; i < 8; i++) {

                        // Check if possible in box.
                        if (cells[i].Repetitions[0].Count == 2)
                            for (int j = i + 1; j < 9; j++)
                                if (HasPair(cells[i].Repetitions[0], cells[j].Repetitions[0], ref nakedPair)) {
                                    bool rowAvailable = i / 3 == j / 3;
                                    bool colAvailable = j - i == 3;
                                    // Go through each cell within box.
                                    for (int k = 0; k < 9; k++) {
                                        if (k == i || k == j) continue; // Skip over cells i and j.       
                                        int kIndex = (boxRow + k / 3) * 9 + boxCol + k % 3;
                                        if (!applied) applied = notes[kIndex, nakedPair.a] || notes[kIndex, nakedPair.b]; 
                                        notes[kIndex, nakedPair.a] = false;
                                        notes[kIndex, nakedPair.b] = false;

                                        // Row || Col
                                        if (rowAvailable || colAvailable) {     // Cell[u] Coords       // Cell[v] Coords
                                            bool skipUVcells = rowAvailable ? ((k == boxCol + i % 3) || (k == boxCol + j % 3))
                                                                            : ((k == boxRow + i / 3) || (k == boxRow + j / 3));
                                            if (skipUVcells) continue;
                                            int rcIndex = rowAvailable ? ((boxRow + i / 3) * 9 + k) : (k * 9 + boxCol + i % 3);
                                            if (!applied) applied = notes[rcIndex, nakedPair.a] || notes[rcIndex, nakedPair.b];
                                            notes[rcIndex, nakedPair.a] = false;
                                            notes[rcIndex, nakedPair.b] = false;
                                        }
                                    }

                                    if (applied) {
                                        if (LogConsole) {
                                            s.Append($"[{nakedPair.a + 1}, {nakedPair.b + 1}] found on: \n");
                                            s.AppendLine($"Box ({boxRow}, {boxCol}) on cells ({boxRow + i / 3}, {boxCol + i % 3}) and ({boxRow + j / 3}, {boxCol + j % 3})");
                                            if (rowAvailable) s.AppendLine($"Row ({row}) as well.");
                                            else if (colAvailable) s.AppendLine($"Col ({col}) as well.");
                                            Debug.Log(s.ToString());
                                        }
                                        TimesUsed++;
                                        cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                        return true;
                                    } // => log entries && return true;
                                }

                        // Check if possible in row and column.
                        if (cells[i].Repetitions[1].Count == 2 || cells[i].Repetitions[2].Count == 2)
                            for (int rank = 0; rank < 2; rank++)
                                for (int j = i + 1; j < 9; j++) {
                                    if (rank == 0 ? HasPair(cells[i].Repetitions[1], cells[j].Repetitions[1], ref nakedPair) 
                                                  : HasPair(cells[i].Repetitions[2], cells[j].Repetitions[2], ref nakedPair)) {
                                        for (int k = 0; k < 9; k++) {
                                            if (k == i || k == j) continue;
                                            int rcIndex = rank == 0 ? (row * 9 + k) : (k * 9 + col);
                                            if (!applied) applied = notes[rcIndex, nakedPair.a] || notes[rcIndex, nakedPair.b];
                                            notes[rcIndex, nakedPair.a] = false;
                                            notes[rcIndex, nakedPair.b] = false;
                                        }

                                        if (applied) {                                  
                                            TimesUsed++;
                                            cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                            if (LogConsole) {
                                                s.Append($"[{nakedPair.a + 1}, {nakedPair.b + 1}] found on: \n");
                                                s.AppendLine(i == 0 ? $"Row ({row}) on cells ({row}, {i}) and ({row}, {j})." :
                                                                      $"Col ({col}) on cells ({i}, {col}) and ({j}, {col}).");
                                                Debug.Log(s.ToString());
                                            }
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
        private bool HasPair(List<int> l1, List<int> l2, ref (int, int) pair)
        {
            if (l1.Count == 2 && l2.Count == 2) {
                var union = l1.Union(l2).ToList();
                if (union.Count() == 2) {
                    pair = (union[0], union[1]);
                    return true;
                }
            }

            return false;
        }
    }
}