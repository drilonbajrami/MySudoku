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
    public class NakedTriples : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 2000;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1400;

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
                            if (notes[kRow * 9 + kCol, i]) cells[k].Box.Add(i);
                            if (notes[row * 9 + k, i]) cells[k].Row.Add(i);
                            if (notes[k * 9 + col, i]) cells[k].Col.Add(i);
                        }
                    }

                    (int a, int b, int c) nakedTriple = (-1, -1, -1);
                    bool applied = false;
                    StringBuilder log = new("Naked Triple ");
                    // Go through combinations of triples.
                    for (int p = 0; p < 7; p++) {

                        // Scan box.
                        if (cells[p].Box.Count == 2 || cells[p].Box.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Box.Count == 2 || cells[u].Box.Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (cells[v].Box.Count == 2 || cells[v].Box.Count == 3) // (_, _, v)
                                            if (HasTriple(cells[p].Box, cells[u].Box, cells[v].Box, ref nakedTriple)) {

                                                bool rowAvailable = p / 3 == u / 3 && p / 3 == v / 3;
                                                bool colAvailable = u - p == 3 && v - u == 3;
                                                for (int k = 0; k < 9; k++) {
                                                    if (k == p || k == u || k == v) continue;

                                                    int c1 = (boxRow + k / 3) * 9 + boxCol + k % 3;
                                                    int c2 = (boxRow + k / 3) * 9 + boxCol + k % 3;
                                                    int c3 = (boxRow + k / 3) * 9 + boxCol + k % 3;
                                                    if (!applied) applied = notes[c1, nakedTriple.a] || notes[c2, nakedTriple.b] || notes[c3, nakedTriple.c];
                                                    notes[c1, nakedTriple.a] = false;
                                                    notes[c2, nakedTriple.b] = false;
                                                    notes[c3, nakedTriple.c] = false;

                                                    if (rowAvailable || colAvailable) {
                                                        bool skipPUVcells = rowAvailable ? ((k == boxCol + p % 3) || (k == boxCol + u % 3) || (k == boxCol + v % 3))
                                                                                         : ((k == boxRow + p / 3) || (k == boxRow + u / 3) || (k == boxRow + v / 3));
                                                        if (skipPUVcells) continue;
                                                        int rcIndex = rowAvailable ? ((boxRow + p / 3) * 9 + k) : (k * 9 + boxCol + p % 3);
                                                        if (!applied) applied = notes[rcIndex, nakedTriple.a] || notes[rcIndex, nakedTriple.b] || notes[rcIndex, nakedTriple.c];
                                                        notes[rcIndex, nakedTriple.a] = false;
                                                        notes[rcIndex, nakedTriple.b] = false;
                                                        notes[rcIndex, nakedTriple.c] = false;
                                                    }
                                                }

                                                if (applied) {
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
                        if (cells[p].Row.Count == 2 || cells[p].Row.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Row.Count == 2 || cells[u].Row.Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (HasTriple(cells[p].Row, cells[u].Row, cells[v].Row, ref nakedTriple)) {
                                            for (int k = 0; k < 9; k++) {
                                                if (k == p || k == u || k == v) continue;
                                                int c = row * 9 + k;
                                                if (!applied) applied = notes[c, nakedTriple.a] || notes[c, nakedTriple.b] || notes[c, nakedTriple.c];
                                                notes[c, nakedTriple.a] = false;
                                                notes[c, nakedTriple.b] = false;
                                                notes[c, nakedTriple.c] = false;
                                            }

                                            if (applied) {
                                                if (!LogConsole) return true;
                                                log.Append($"[{nakedTriple.a + 1}, {nakedTriple.b + 1}, {nakedTriple.c + 1}] found on: \n");
                                                log.AppendLine($"Row ({row}) on cells ({row}, {p}), ({row}, {u}) and ({row}, {v})");
                                                Debug.Log(log.ToString());
                                                return true;
                                            } // => log entries && return true;
                                        }

                        // Row
                        if (cells[p].Col.Count == 2 || cells[p].Col.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Col.Count == 2 || cells[u].Col.Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (HasTriple(cells[p].Col, cells[u].Col, cells[v].Col, ref nakedTriple)) {
                                            for (int k = 0; k < 9; k++) {
                                                if (k == p || k == u || k == v) continue;
                                                int c = k * 9 + col;
                                                if (!applied) applied = notes[c, nakedTriple.a] || notes[c, nakedTriple.b] || notes[c, nakedTriple.c];
                                                notes[c, nakedTriple.a] = false;
                                                notes[c, nakedTriple.b] = false;
                                                notes[c, nakedTriple.c] = false;
                                            }

                                            if (applied) {
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