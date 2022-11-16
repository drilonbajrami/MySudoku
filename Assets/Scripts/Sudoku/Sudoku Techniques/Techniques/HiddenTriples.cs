using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            cost = 0;
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