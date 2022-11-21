using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// More info on: https://www.sudokuoftheday.com/techniques/hidden-pairs-triples.
    /// </summary>
    public class HiddenTriples : ISudokuTechnique
    {
        /// <inheritdoc/>
        public int TimesUsed { get; set; } = 0;

        /// <inheritdoc/>
        public int FirstUseCost => 2400;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1600;

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
            StringBuilder s = new("Hidden Triple ");
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

                    (int a, int b, int c) cellTriplet = (-1, -1, -1);
                    bool applied = false;
                    for (int i = 0; i < 7; i++) {

                        // Check if possible in box.
                        if (candidates[i].Repetitions[0].Count == 2 || candidates[i].Repetitions[0].Count == 3)
                            for (int j = i + 1; j < 8; j++)
                                if (candidates[j].Repetitions[0].Count == 2 || candidates[j].Repetitions[0].Count == 3)
                                    for (int k = j + 1; k < 9; k++)
                                        if (HasTriple(candidates[i].Repetitions[0], candidates[j].Repetitions[0], candidates[k].Repetitions[0], ref cellTriplet)) {
                                            (int a, int b, int c) triplet = ((boxRow + cellTriplet.a / 3) * 9 + boxCol + cellTriplet.a % 3,
                                                                             (boxRow + cellTriplet.b / 3) * 9 + boxCol + cellTriplet.b % 3,
                                                                             (boxRow + cellTriplet.c / 3) * 9 + boxCol + cellTriplet.c % 3);

                                            for(int n = 0; n < 9; n++) {
                                                if (n == i || n == j || n == k) continue;
                                                if (!applied) applied = notes[triplet.a, n] || notes[triplet.b, n] || notes[triplet.c, n];
                                                notes[triplet.a, n] = false;
                                                notes[triplet.b, n] = false;
                                                notes[triplet.c, n] = false;
                                            }

                                            if (applied) {
                                                TimesUsed++;
                                                cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                                if (LogConsole) {
                                                    s.Append($"[{i + 1}, {j + 1}, {k + 1}] found on: \n");
                                                    s.AppendLine($"Box ({boxRow}, {boxCol}) on cells " +
                                                                 $"({boxRow + cellTriplet.a / 3}, {boxCol + cellTriplet.a % 3}), " +
                                                                 $"({boxRow + cellTriplet.b / 3}, {boxCol + cellTriplet.b % 3}) and " +
                                                                 $"({boxRow + cellTriplet.c / 3}, {boxCol + cellTriplet.c % 3})");

                                                    for (int q = 0; q < 9; q++) {
                                                        if (q == i || q == j || q == k) {
                                                            s.Append($"Candidate [{q + 1}] appears on cells ");
                                                            for (int p = 0; p < candidates[q].Repetitions[0].Count; p++)
                                                                s.Append($"({boxRow + candidates[q].Repetitions[0][p] / 3}, {boxCol + candidates[q].Repetitions[0][p] % 3}), ");
                                                            s.Append("\n");
                                                        }
                                                    }
                                                    Debug.Log(s.ToString());
                                                }
                                                return true;
                                            } // => log entries && return true;
                                        }

                        // Check if possible in row.
                        if (candidates[i].Repetitions[1].Count == 2 || candidates[i].Repetitions[1].Count == 3)
                            for (int j = i + 1; j < 8; j++)
                                if (candidates[j].Repetitions[1].Count == 2 || candidates[j].Repetitions[1].Count == 3)
                                    for (int k = j + 1; k < 9; k++)
                                        if (HasTriple(candidates[i].Repetitions[1], candidates[j].Repetitions[1], candidates[k].Repetitions[1], ref cellTriplet)) {
                                            (int a, int b, int c) triplet = (row * 9 + cellTriplet.a, 
                                                                             row * 9 + cellTriplet.b,
                                                                             row * 9 + cellTriplet.c);

                                            for (int n = 0; n < 9; n++) {
                                                if (n == i || n == j || n == k) continue;
                                                if (!applied) applied = notes[triplet.a, n] || notes[triplet.b, n] || notes[triplet.c, n];
                                                notes[triplet.a, n] = false;
                                                notes[triplet.b, n] = false;
                                                notes[triplet.c, n] = false;
                                            }

                                            if (applied) {
                                                TimesUsed++;
                                                cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                                if (LogConsole) {
                                                    s.Append($"[{i + 1}, {j + 1}, {k + 1}] found on: \n");
                                                    s.AppendLine($"Row ({row}) on cells " +
                                                                 $"({row}, {cellTriplet.a}), ({row}, {cellTriplet.b}) and ({row}, {cellTriplet.c})");
                                                    for(int q = 0; q < 9; q++) {
                                                        if(q == i || q == j || q == k) {
                                                            s.Append($"Candidate [{q + 1}] appears on cells ");
                                                            for (int p = 0; p < candidates[q].Repetitions[1].Count; p++)
                                                                s.Append($"({row}, {candidates[q].Repetitions[1][p]}), ");
                                                            s.Append("\n");
                                                         }
                                                    }
                                                    Debug.Log(s.ToString());
                                                }
                                                return true;
                                            } // => log entries && return true;
                                        }

                        // Check if possible in column.
                        if (candidates[i].Repetitions[2].Count == 2 || candidates[i].Repetitions[2].Count == 3)
                            for (int j = i + 1; j < 8; j++)
                                if (candidates[j].Repetitions[2].Count == 2 || candidates[j].Repetitions[2].Count == 3)
                                    for (int k = j + 1; k < 9; k++)
                                        if (HasTriple(candidates[i].Repetitions[2], candidates[j].Repetitions[2], candidates[k].Repetitions[2], ref cellTriplet)) {
                                            (int a, int b, int c) triplet = (cellTriplet.a * 9 + col,
                                                                             cellTriplet.b * 9 + col,
                                                                             cellTriplet.c * 9 + col);

                                            for (int n = 0; n < 9; n++) {
                                                if (n == i || n == j || n == k) continue;
                                                if (!applied) applied = notes[triplet.a, n] || notes[triplet.b, n] || notes[triplet.c, n];
                                                notes[triplet.a, n] = false;
                                                notes[triplet.b, n] = false;
                                                notes[triplet.c, n] = false;
                                            }

                                            if (applied) {
                                                TimesUsed++;
                                                cost = TimesUsed == 1 ? FirstUseCost : SubsequentUseCost;
                                                if (LogConsole) {
                                                    s.Append($"[{i + 1}, {j + 1}, {k + 1}] found on: \n");
                                                    s.AppendLine($"Col ({col}) on cells ({cellTriplet.a}, {col}), ({cellTriplet.b}, {col}) and ({cellTriplet.c}, {col})");
                                                    for (int q = 0; q < 9; q++) {
                                                        if (q == i || q == j || q == k) {
                                                            s.Append($"Candidate [{q + 1}] appears on cells ");
                                                            for (int p = 0; p < candidates[q].Repetitions[2].Count; p++)
                                                                s.Append($"({candidates[q].Repetitions[2][p]}, {col}), ");
                                                            s.Append("\n");
                                                        }
                                                    }
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
        /// Checks if the union of all three sets contains three elements, and if one of the sets contains <br/>
        /// one of the other, based on their counts.
        /// </summary>
        /// <param name="s1">First set.</param>
        /// <param name="s2">Second set.</param>
        /// <param name="s3">Third set.</param>
        /// <returns>Whether there union of all the sets contains a triple.</returns>
        private bool HasTriple(List<int> s1, List<int> s2, List<int> s3, ref (int, int, int) triple)
        {
            bool check = (s1.Count == 2 || s1.Count == 3) && (s2.Count == 2 || s2.Count == 3) && (s3.Count == 2 || s3.Count == 3);
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