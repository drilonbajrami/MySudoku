using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Hidden Pairs technique is applicable when a pair of candidates appears only on two cells within a box, row or column.
    /// However, this candidate pair is present with other candidates in those cells as well, which means those other candidates
    /// can be removed from those two cells.
    /// More info on: https://www.sudokuoftheday.com/techniques/hidden-pairs-triples.
    /// </summary>
    public class HiddenPairs : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 1500;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1200;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
            Repetition[] candidates = new Repetition[9] {
                new Repetition(0), new Repetition(1), new Repetition(2),
                new Repetition(3), new Repetition(4), new Repetition(5),
                new Repetition(6), new Repetition(7), new Repetition(8)
            };

            StringBuilder s = new("Naked Pair ");
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
                for (int boxCol = 0; boxCol < 9; boxCol += 3) {
                    // Clear are previous data from the candidate's repetitions.
                    for (int index = 0; index < 9; index++) candidates[index].ClearAll();
                    int row = boxRow + boxCol / 3;
                    int col = boxRow / 3 + boxCol;

                    for (int n = 0; n < 9; n++) // Candidate's index as [i].
                        for (int k = 0; k < 9; k++) { // Cell's index as [k].
                            if (notes[(boxRow + k / 3) * 9 + boxCol + k % 3, n] && sudoku[boxRow + k / 3, boxCol + k % 3] == 0) candidates[n].Box.Add(k);
                            if (notes[row * 9 + k, n] && sudoku[row, k] == 0) candidates[n].Row.Add(k);
                            if (notes[k * 9 + col, n] && sudoku[k, col] == 0) candidates[n].Col.Add(k);
                        }

                    bool applied = false;
                    for (int i = 0; i < 8; i++) {

                        // Check if possible in box.
                        if (candidates[i].Box.Count == 2)
                            for (int j = i + 1; j < 9; j++)
                                if (HasPair(candidates[i].Box, candidates[j].Box)) {
                                    (int c1, int c2) pair = ((boxRow + candidates[i].Box[0] / 3) * 9 + boxCol + candidates[i].Box[0] % 3,
                                                             (boxRow + candidates[i].Box[1] / 3) * 9 + boxCol + candidates[i].Box[1] % 3);

                                    for (int n = 0; n < 9; n++) {
                                        if (n == i || n == j) continue;
                                        if (!applied) applied = notes[pair.c1, n] || notes[pair.c2, n];
                                        notes[pair.c1, n] = false;
                                        notes[pair.c2, n] = false;
                                    }

                                    if (applied) {
                                        if (!LogConsole) return true;
                                        s.Append($"[{i + 1}, {j + 1}] found on: \n");
                                        s.AppendLine($"Box ({boxRow}, {boxCol}) on cells " +
                                            $"({boxRow + candidates[i].Box[0] / 3}, {boxCol + candidates[i].Box[0] % 3}) and " +
                                            $"({boxRow + candidates[i].Box[1] / 3}, {boxCol + candidates[i].Box[0] % 3})");
                                        Debug.Log(s.ToString());
                                        return true;
                                    } // => log entries && return true;
                                }

                        // Check if possible in row or column.
                        if (candidates[i].Row.Count == 2 || candidates[i].Col.Count == 2) {
                            for (int rank = 0; rank < 2; rank++)
                                for (int j = i + 1; j < 9; j++)
                                    if (rank == 0 ? HasPair(candidates[i].Row, candidates[j].Row) : HasPair(candidates[i].Col, candidates[j].Col)) {
                                        int c1 = rank == 0 ? (row * 9 + candidates[i].Row[0]) : (candidates[i].Col[0] * 9 + col);
                                        int c2 = rank == 0 ? (row * 9 + candidates[i].Row[1]) : (candidates[i].Col[1] * 9 + col);
                                        for (int n = 0; n < 9; n++) {
                                            if (n == i || n == j) continue;
                                            if (!applied) applied = notes[c1, n] || notes[c2, n];
                                            notes[c1, n] = false;
                                            notes[c2, n] = false;
                                        }

                                        if (applied) {
                                            if (!LogConsole) return true;
                                            s.Append($"[{i + 1}, {j + 1}] found on: \n");
                                            s.AppendLine(rank == 0 ? $"Row ({row}) on cells ({row}, {candidates[i].Row[0]}) and ({row}, {candidates[i].Row[1]})." :
                                                                     $"Col ({col}) on cells ({candidates[i].Col[0]}, {col}) and ({candidates[i].Col[1]}, {col}).");
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