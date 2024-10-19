using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/naked-pairs-triples.
    /// </summary>
    public class NakedTriples : SudokuTechnique
    {
        protected override int FirstUseCost => 2000;
        protected override int SubsequentUseCost => 1400;

        public override bool Apply(int[,] sudoku, bool[,,] notes, out int cost)
        {
            int numberOfSets = 3;
            Repetition[] cells = new Repetition[9] {
                new Repetition(0, numberOfSets), new Repetition(1, numberOfSets), new Repetition(2, numberOfSets),
                new Repetition(3, numberOfSets), new Repetition(4, numberOfSets), new Repetition(5, numberOfSets),
                new Repetition(6, numberOfSets), new Repetition(7, numberOfSets), new Repetition(8, numberOfSets)
            };

            cost = 0;
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
                for (int boxCol = 0; boxCol < 9; boxCol += 3) {
                    // Clear are previous data from the candidate's repetitions.
                    for (int i = 0; i < 9; i++) cells[i].ClearAll();

                    // Store the row and column to scan.
                    int row = boxRow + boxCol / 3;
                    int col = boxRow / 3 + boxCol;

                    for (int k = 0; k < 9; k++) {
                        int kRow = boxRow + k / 3;

                        int kCol = boxCol + k % 3;
                        for (int i = 0; i < 9; i++) {
                            if (notes[kRow, kCol, i]) cells[k].Repetitions[0].Add(i);
                            if (notes[row, k, i]) cells[k].Repetitions[1].Add(i);
                            if (notes[k, col, i]) cells[k].Repetitions[2].Add(i);
                        }
                    }

                    (int a, int b, int c) nakedTriple = (-1, -1, -1);
                    bool applied = false;
                    StringBuilder log = new("Naked Triple ");
                    // Go through combinations of triples.
                    for (int p = 0; p < 7; p++) {

                        // Scan box.
                        if (cells[p].Repetitions[0].Count == 2 || cells[p].Repetitions[0].Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Repetitions[0].Count == 2 || cells[u].Repetitions[0].Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (HasTriple(cells[p].Repetitions[0], cells[u].Repetitions[0], cells[v].Repetitions[0], ref nakedTriple)) {
                                            bool rowAvailable = p / 3 == u / 3 && p / 3 == v / 3;
                                            bool colAvailable = u - p == 3 && v - u == 3;

                                            // Go through each cell and deactivate all other candidates but the ones from the naked triple.
                                            for (int k = 0; k < 9; k++) {
                                                if (k == p || k == u || k == v) continue;

                                                (int row, int col) c = (boxRow + k / 3, boxCol + k % 3);
                                                if (!applied) applied = notes[c.row, c.col, nakedTriple.a] || notes[c.row, c.col, nakedTriple.b] || notes[c.row, c.col, nakedTriple.c];
                                                notes[c.row, c.col, nakedTriple.a] = false;
                                                notes[c.row, c.col, nakedTriple.b] = false;
                                                notes[c.row, c.col, nakedTriple.c] = false;

                                                if (rowAvailable || colAvailable) {
                                                    bool skipPUVCells = rowAvailable ? (k == boxCol + p % 3 || k == boxCol + u % 3 || k == boxCol + v % 3)
                                                                                     : (k == boxRow + p / 3 || k == boxRow + u / 3 || k == boxRow + v / 3);
                                                    if (skipPUVCells) continue;
                                                    c = rowAvailable ? (boxCol + p % 3, k) : (k, boxRow + p / 3);
                                                    if (!applied) applied = notes[c.row, c.col, nakedTriple.a] || notes[c.row, c.col, nakedTriple.b] || notes[c.row, c.col, nakedTriple.c];
                                                    notes[c.row, c.col, nakedTriple.a] = false;
                                                    notes[c.row, c.col, nakedTriple.b] = false;
                                                    notes[c.row, c.col, nakedTriple.c] = false;
                                                }
                                            }

                                            if (applied) {
                                                cost = GetUsageCost();
                                                if (!LogConsole) return true;
                                                log.Append($"[{nakedTriple.a + 1}, {nakedTriple.b + 1}, {nakedTriple.c + 1}] found on: \n");
                                                log.AppendLine($"Box ({boxRow}, {boxCol}) on cells ({boxRow + p / 3}, {boxCol + p % 3}), " +
                                                             $"({boxRow + u / 3}, {boxCol + u % 3}) and ({boxRow + v / 3}, {boxCol + v % 3})");
                                                if (rowAvailable) log.AppendLine($"Row ({row}) as well.");
                                                else if (colAvailable) log.AppendLine($"Col ({col}) as well.");
                                                Debug.Log(log.ToString());
                                                return true;
                                            } // => log entries && return true;
                                        }

                        // Row
                        if (cells[p].Repetitions[1].Count == 2 || cells[p].Repetitions[1].Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Repetitions[1].Count == 2 || cells[u].Repetitions[1].Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (HasTriple(cells[p].Repetitions[1], cells[u].Repetitions[1], cells[v].Repetitions[1], ref nakedTriple)) {
                                            for (int k = 0; k < 9; k++) {
                                                if (k == p || k == u || k == v) continue;
                                                if (!applied) applied = notes[row, k, nakedTriple.a] || notes[row, k, nakedTriple.b] || notes[row, k, nakedTriple.c];
                                                notes[row, k, nakedTriple.a] = false;
                                                notes[row, k, nakedTriple.b] = false;
                                                notes[row, k, nakedTriple.c] = false;
                                            }

                                            if (applied) {
                                                cost = GetUsageCost();
                                                if (!LogConsole) return true;
                                                log.Append($"[{nakedTriple.a + 1}, {nakedTriple.b + 1}, {nakedTriple.c + 1}] found on: \n");
                                                log.AppendLine($"Row ({row}) on cells ({row}, {p}), ({row}, {u}) and ({row}, {v})");
                                                Debug.Log(log.ToString());
                                                return true;
                                            } // => log entries && return true;
                                        }

                        // Col
                        if (cells[p].Repetitions[2].Count == 2 || cells[p].Repetitions[2].Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Repetitions[2].Count == 2 || cells[u].Repetitions[2].Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (HasTriple(cells[p].Repetitions[2], cells[u].Repetitions[2], cells[v].Repetitions[2], ref nakedTriple)) {
                                            for (int k = 0; k < 9; k++) {
                                                if (k == p || k == u || k == v) continue;
                                                if (!applied) applied = notes[k, col, nakedTriple.a] || notes[k, col, nakedTriple.b] || notes[k, col, nakedTriple.c];
                                                notes[k, col, nakedTriple.a] = false;
                                                notes[k, col, nakedTriple.b] = false;
                                                notes[k, col, nakedTriple.c] = false;
                                            }

                                            if (applied) {
                                                cost = GetUsageCost();
                                                if (!LogConsole) return true;
                                                log.Append($"[{nakedTriple.a + 1}, {nakedTriple.b + 1}, {nakedTriple.c + 1}] found on: \n");
                                                log.AppendLine($"Col ({col}) on cells ({p}, {col}), ({u}, {col}) and ({v}, {col})");
                                                Debug.Log(log.ToString());
                                                return true;
                                            } // => log entries && return true;
                                        }
                    }
                }

            return false;
        }

        /// <summary>
        /// Checks if the union of all three sets contains three elements, and if one of the sets contains <br/>
        /// one of the other, based on their counts.
        /// </summary>
        /// <param name="s1">First set.</param>
        /// <param name="s2">Second set.</param>
        /// <param name="s3">Third set.</param>
        /// <returns>Whether there union of all the sets contains a triple.</returns>
        private bool HasTriple(List<int> s1, List<int> s2, List<int> s3, ref (int, int, int) triple)
        {
            bool check = (s1.Count == 2 || s1.Count == 3) && (s2.Count == 2 || s2.Count == 3) || (s3.Count == 2 || s3.Count == 3);
            if (check) {
                List<int> union = s1.Union(s2).Union(s3).ToList();
                if (union.Count == 3) {
                    triple = (union[0], union[1], union[2]);
                    return true;
                }
            }

            return false;
        }
    }
}