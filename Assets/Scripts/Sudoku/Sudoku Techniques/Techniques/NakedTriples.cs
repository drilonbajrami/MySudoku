using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                    bool applied = false;

                    // Go through combinations of triples.
                    for (int p = 0; p < 7; p++) {

                        // Scan box.
                        if (cells[p].Box.Count == 2 || cells[p].Box.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Box.Count == 2 || cells[u].Box.Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (cells[v].Box.Count == 2 || cells[v].Box.Count == 3) // (_, _, v)
                                            if (HasTriple(cells[p].Box, cells[u].Box, cells[v].Box)) {
                                                Debug.Log($"[{cells[p].Index + 1}, {cells[u].Index + 1}, {cells[v].Index + 1}] " +
                                                    $"is a naked triple in Box({boxRow}, {boxCol}).");

                                                return true;
                                            }

                        // Row
                        if (cells[p].Row.Count == 2 || cells[p].Row.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Row.Count == 2 || cells[u].Row.Count == 3)  // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (cells[v].Row.Count == 2 || cells[v].Row.Count == 3) // (_, _, v)
                                            if (HasTriple(cells[p].Row, cells[u].Row, cells[v].Row)) {
                                                Debug.Log($"[{cells[p].Index + 1}, {cells[u].Index + 1}, {cells[v].Index + 1}] " +
                                                    $"is a naked triple in Row({row}).");

                                                return true;
                                            }

                        // Col
                        if (cells[p].Col.Count == 2 || cells[p].Col.Count == 3) // (p, _, _)
                            for (int u = p + 1; u < 8; u++)
                                if (cells[u].Col.Count == 2 || cells[u].Col.Count == 3) // (_, u, _)
                                    for (int v = u + 1; v < 9; v++)
                                        if (cells[v].Col.Count == 2 || cells[v].Col.Count == 3) // (_, _, v)
                                            if (HasTriple(cells[p].Col, cells[u].Col, cells[v].Col)) {
                                                Debug.Log($"[{cells[p].Index + 1}, {cells[u].Index + 1}, {cells[v].Index + 1}] " +
                                                    $"is a naked triple in Col({col}).");

                                                return true;
                                            }

                    }
                }

            return false;
        }

        private bool HasTriple(List<int> c1, List<int> c2, List<int> c3)
        {
            int count = c1.Count + c2.Count + c3.Count;

            if (count == 9) return c1.All(c2.Contains) && c2.All(c3.Contains);
            else if (count == 8) return (c1.Count == c2.Count && c1.All(c2.Contains) && c3.All(c1.Contains)) ||
                                        (c1.Count == c3.Count && c1.All(c3.Contains) && c2.All(c1.Contains)) ||
                                        (c2.Count == c3.Count && c2.All(c3.Contains) && c1.All(c2.Contains));
            else if (count == 7) return (c1.Count == c2.Count && c1.All(c3.Contains) && c2.All(c3.Contains)) ||
                                        (c1.Count == c3.Count && c1.All(c2.Contains) && c3.All(c2.Contains)) ||
                                        (c2.Count == c3.Count && c2.All(c1.Contains) && c3.All(c1.Contains));
            else return c1.Union(c2).Union(c3).ToList().Count == 3;
        }
    }
}