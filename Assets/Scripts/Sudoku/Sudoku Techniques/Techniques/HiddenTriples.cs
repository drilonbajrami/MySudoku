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
        public int TimesUsed { get; set; }

        /// <inheritdoc/>
        public int FirstUseCost => 2400;

        /// <inheritdoc/>
        public int SubsequentUseCost => 1600;

        /// <inheritdoc/>
        public bool LogConsole { get; set; } = false;

        /// <inheritdoc/>
        public bool ApplyTechnique(int[,] sudoku, bool[,] notes)
        {
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