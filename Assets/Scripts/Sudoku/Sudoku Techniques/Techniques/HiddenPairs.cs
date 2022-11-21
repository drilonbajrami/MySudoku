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
        public int TimesUsed { get; set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 1500;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1200;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes, out int cost)
        {
            int numberOfSets = 3;
            Repetition[] candidates = new Repetition[9] {
                new Repetition(0, numberOfSets), new Repetition(1, numberOfSets), new Repetition(2, numberOfSets),
                new Repetition(3, numberOfSets), new Repetition(4, numberOfSets), new Repetition(5, numberOfSets),
                new Repetition(6, numberOfSets), new Repetition(7, numberOfSets), new Repetition(8, numberOfSets)
            };

            cost = 0;
            StringBuilder s = new("Hidden Pair ");
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
                for (int boxCol = 0; boxCol < 9; boxCol += 3) {
                    // Clear are previous data from the candidate's repetitions.
                    for (int index = 0; index < 9; index++) candidates[index].ClearAll();
                    int row = boxRow + boxCol / 3;
                    int col = boxRow / 3 + boxCol;

                    for (int n = 0; n < 9; n++) // Candidate's index as [i].
                        for (int k = 0; k < 9; k++) { // Cell's index as [k].
                            if (notes[(boxRow + k / 3) * 9 + boxCol + k % 3, n] && sudoku[boxRow + k / 3, boxCol + k % 3] == 0) candidates[n].Repetitions[0].Add(k);
                            if (notes[row * 9 + k, n] && sudoku[row, k] == 0) candidates[n].Repetitions[1].Add(k);
                            if (notes[k * 9 + col, n] && sudoku[k, col] == 0) candidates[n].Repetitions[2].Add(k);
                        }

                    (int a, int b) cellPair = (-1, -1);
                    bool applied = false;
                    for (int i = 0; i < 8; i++) {
                        // Check if possible in box.
                        if (candidates[i].Repetitions[0].Count == 2)
                            for (int j = i + 1; j < 9; j++)
                                if (HasPair(candidates[i].Repetitions[0], candidates[j].Repetitions[0], ref cellPair)) {
                                    
                                    (int a, int b) pair = ((boxRow + cellPair.a / 3) * 9 + boxCol + cellPair.a % 3,
                                                           (boxRow + cellPair.b / 3) * 9 + boxCol + cellPair.b % 3);
                                    for (int n = 0; n < 9; n++) {
                                        if (n == i || n == j) continue;
                                        if (!applied) applied = notes[pair.a, n] || notes[pair.b, n];
                                        notes[pair.a, n] = false;
                                        notes[pair.b, n] = false;
                                    }

                                    if (applied) {
                                        TimesUsed++;
                                        cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                        if (LogConsole) {
                                            s.Append($"[{i + 1}, {j + 1}] found on: \n");
                                            s.AppendLine($"Box ({boxRow}, {boxCol}) on cells " +
                                                         $"({boxRow + cellPair.a / 3}, {boxCol + cellPair.a % 3}) and " +
                                                         $"({boxRow + cellPair.b / 3}, {boxCol + cellPair.b % 3})");
                                            Debug.Log(s.ToString());
                                        }
                                        return true;
                                    } // => log entries && return true;
                                }

                        // Check if possible in row or column.
                        if (candidates[i].Repetitions[1].Count == 2 || candidates[i].Repetitions[2].Count == 2) 
                            for (int rank = 0; rank < 2; rank++)
                                for (int j = i + 1; j < 9; j++)
                                    if (rank == 0 ? HasPair(candidates[i].Repetitions[1], candidates[j].Repetitions[1], ref cellPair)
                                                  : HasPair(candidates[i].Repetitions[2], candidates[j].Repetitions[2], ref cellPair)) {
                                        // Apply technique.
                                        int c1 = rank == 0 ? (row * 9 + cellPair.a) : (cellPair.a * 9 + col);
                                        int c2 = rank == 0 ? (row * 9 + cellPair.b) : (cellPair.b * 9 + col);
                                        for (int n = 0; n < 9; n++) {
                                            if (n == i || n == j) continue;
                                            if (!applied) applied = notes[c1, n] || notes[c2, n];
                                            notes[c1, n] = false;
                                            notes[c2, n] = false;
                                        }

                                        if (applied) {
                                            TimesUsed++;
                                            cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                            if (LogConsole) {
                                                s.Append($"[{i + 1}, {j + 1}] found on: \n");
                                                s.AppendLine(rank == 0 ? $"Row ({row}) on cells ({row}, {candidates[i].Repetitions[1][0]}) and ({row}, {candidates[i].Repetitions[1][1]})." :
                                                                         $"Col ({col}) on cells ({candidates[i].Repetitions[2][0]}, {col}) and ({candidates[i].Repetitions[2][1]}, {col}).");
                                                Debug.Log(s.ToString());
                                            }
                                            return true;
                                        } // => log entries && return true;
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