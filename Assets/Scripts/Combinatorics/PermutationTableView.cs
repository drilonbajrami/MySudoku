using UnityEngine;
using System;
using MySudoku;
using System.Collections.Generic;

namespace SudokuTesting
{
    /// <summary>
    /// Visualizes all the possible three number permutations of range from 1 to 9, in a readable table format.
    /// </summary>
    public class PermutationTableView : MonoBehaviour
    {
        /// <summary>
        /// Permutation prefab.
        /// </summary>
        [SerializeField] private Permutation _permutationPrefab;
        /// <summary>
        /// Cache the spawned permutations.
        /// </summary>
        private Permutation[,,] _permutations;
        /// <summary>
        /// Keep track of checked permutations, used for unchecking them later.
        /// </summary>
        private readonly Stack<Permutation> _checkedPermutations = new();

        /// <summary>
        /// Spawns the permutations table and checks the permutations if table is shown.
        /// </summary>
        /// <param name="grid">The grid to check the permutations for.</param>
        /// <param name="show">Whether to show the permutation table view or not.</param>
        public void Initialize(Cell[,] grid, bool show)
        {
            gameObject.SetActive(show);
            if(_permutations == null) {
                _permutations = new Permutation[9, 8, 7];
                SpawnPermutations();
            }

            if (show) CheckPermutations(grid);
        }

        /// <summary>
        /// Spawns the permutations in table format.
        /// </summary>
        private void SpawnPermutations()
        {
            float l = _permutationPrefab.RectTransform.sizeDelta.x + 3;
            float h = _permutationPrefab.RectTransform.sizeDelta.y + 3;
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 8; y++) {
                    for (int z = 0; z < 7; z++) {
                        Permutation permutation = Instantiate(_permutationPrefab, transform);
                        permutation.RectTransform.anchoredPosition = new Vector3(z * l + (x / 4) * (7.25f * l),
                                                                                (x % 4 * 8.5f + y) * -h, 0);
                        permutation.SetNumbers(new int[] { SudokuData.Permutations[x, y, z, 0],
                                                      SudokuData.Permutations[x, y, z, 1],
                                                      SudokuData.Permutations[x, y, z, 2] });

                        _permutations[x, y, z] = permutation;
                    }
                }
            }
        }

        /// <summary>
        /// Checks all the used permutations in the given sudoku grid of cells.
        /// </summary>
        /// <param name="grid">The grid to check the permutations for.</param>
        /// <returns></returns>
        public void CheckPermutations(Cell[,] grid)
        {
            UncheckPermutations();
            int[] permutation = new int[3];
            for (int box = 0; box < 9; box++) {
                int row = box / 3 * 3;
                int col = box % 3 * 3;

                for (int i = 0; i < 3; i++) {
                    // Horizontal direction
                    permutation[0] = grid[row + i, col].Number;
                    permutation[1] = grid[row + i, col + 1].Number;
                    permutation[2] = grid[row + i, col + 2].Number;

                    if (permutation[0] != 0 && permutation[1] != 0 && permutation[2] != 0)
                        CheckPermutation(permutation, true, box + 1);

                    // Vertical direction
                    permutation[0] = grid[row, col + i].Number;
                    permutation[1] = grid[row + 1, col + i].Number;
                    permutation[2] = grid[row + 2, col + i].Number;

                    if (permutation[0] != 0 && permutation[1] != 0 && permutation[2] != 0)
                        CheckPermutation(permutation, false, box + 1);
                }
            }
        }

        /// <summary>
        /// Checks/marks the given permutation.
        /// </summary>
        /// <param name="permutation">The permutation.</param>
        /// <param name="horizontal">Whether the permutation is of horizontal or vertical direction.</param>
        /// <param name="box">The sudoku box/region in which the permutation is in.</param>
        /// <returns>Whether this permutation has appeared on both directions or not.</returns>
        private void CheckPermutation(int[] permutation, bool horizontal, int box)
        {
            int[] pIndex = SudokuData.FindPermutationIndex(permutation);
            _permutations[pIndex[0], pIndex[1], pIndex[2]].Check(horizontal, box);
            _checkedPermutations.Push(_permutations[pIndex[0], pIndex[1], pIndex[2]]);
        }

        /// <summary>
        /// Unchecks/unmarks all permutations.
        /// </summary>
        private void UncheckPermutations()
        {
            while (_checkedPermutations.Count > 0)
                _checkedPermutations.Pop().Uncheck();
        }
    }
}