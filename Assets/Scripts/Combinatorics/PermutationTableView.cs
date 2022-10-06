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
        [SerializeField] private Permutation _permutationPrefab;
        private Permutation[,,] _permutations = new Permutation[9, 8, 7];
        private Permutation[] _combinations = new Permutation[84];

        private Stack<Permutation> _checkedPermutations = new();

        [SerializeField] private SudokuView _sudokuView;

        public void SpawnTables()
        {
            SpawnPermutations();
            SpawnCombinations();
        }

        public bool CheckPermutations(Cell[,] grid)
        {
            UncheckPermutations();
            bool stop = false;
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
                        stop = CheckPermutation(permutation, true, box + 1, i == 0);

                    // Vertical direction
                    permutation[0] = grid[row, col + i].Number;
                    permutation[1] = grid[row + 1, col + i].Number;
                    permutation[2] = grid[row + 2, col + i].Number;

                    if (permutation[0] != 0 && permutation[1] != 0 && permutation[2] != 0)
                        stop = CheckPermutation(permutation, false, box + 1, i == 0);
                }
            }
            return stop;
        }

        private void Update()
        {
            //CycleThroughNumberPermutations();
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
                        permutation.sudokuView = _sudokuView;
                        permutation.SetDigits(new int[] { SudokuData.Permutations[x, y, z, 0],
                                                      SudokuData.Permutations[x, y, z, 1],
                                                      SudokuData.Permutations[x, y, z, 2] });

                        _permutations[x, y, z] = permutation;
                    }
                }
            }
        }

        /// <summary>
        /// Spawns combinations in table format.
        /// </summary>
        private void SpawnCombinations()
        {
            float l = _permutationPrefab.RectTransform.sizeDelta.x + 3;
            float h = _permutationPrefab.RectTransform.sizeDelta.y + 3;
            for (int y = 0; y < 12; y++) {
                for (int x = 0; x < 7; x++) {
                    Permutation permutation = Instantiate(_permutationPrefab, transform);
                    permutation.RectTransform.anchoredPosition = new Vector3(x * l, y * -h, 0);
                    permutation.SetDigits(new int[] { SudokuData.Combinations[x + y * 7, 0],
                                                  SudokuData.Combinations[x + y * 7, 1],
                                                  SudokuData.Combinations[x + y * 7, 2] });
                    permutation.gameObject.SetActive(false);
                    _combinations[x + y * 7] = permutation;
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
        public bool CheckPermutation(int[] permutation, bool horizontal, int box, bool corner)
        {
            int[] pIndex = SudokuData.FindPermutationIndex(permutation);
            CheckCombination(permutation, horizontal, box);

            // Mark the if current permutation start from a box's top left corner/starting cell.
            // This means that a permutations with starting indexes (row - row % 3, col - col % 3) are present twice
            // in a permutation group.
            if (corner) _permutations[pIndex[0], pIndex[1], pIndex[2]].Corner();
            _checkedPermutations.Push(_permutations[pIndex[0], pIndex[1], pIndex[2]]);
            return _permutations[pIndex[0], pIndex[1], pIndex[2]].Check(horizontal, box);
        }

        public void CheckCombination(int[] permutation, bool horizontal, int box)
        {
            // Check combination of the given permutation.
            int[] comb = new int[3];
            permutation.CopyTo(comb, 0);
            Array.Sort(comb);
            int startIndex = SudokuData.CombinationGroups[comb[0] - 1, 0];
            int endIndex = SudokuData.CombinationGroups[comb[0] - 1, 1] + 1;
            for (int i = startIndex; i < endIndex; i++)
                if (comb[1] == SudokuData.Combinations[i, 1] && comb[2] == SudokuData.Combinations[i, 2])
                    _combinations[i].Check(horizontal, box);
        }

        /// <summary>
        /// Unchecks/unmarks all permutations.
        /// </summary>
        private void UncheckPermutations()
        {
            while (_checkedPermutations.Count > 0)
                _checkedPermutations.Pop().Uncheck();
        }

        private void UncheckCombinations()
        {
            for (int i = 0; i < _combinations.Length; i++)
                _combinations[i].Uncheck();
        }

        /// <summary>
        /// Marks all the permutation for the given numbers based on input from the keypad numbers 1 to 9.
        /// </summary>
        private void MarkPermutationsOnNumberKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
                MarkPermutationsFor(1);
            else if (Input.GetKeyDown(KeyCode.Keypad2))
                MarkPermutationsFor(2);
            else if (Input.GetKeyDown(KeyCode.Keypad3))
                MarkPermutationsFor(3);
            else if (Input.GetKeyDown(KeyCode.Keypad4))
                MarkPermutationsFor(4);
            else if (Input.GetKeyDown(KeyCode.Keypad5))
                MarkPermutationsFor(5);
            else if (Input.GetKeyDown(KeyCode.Keypad6))
                MarkPermutationsFor(6);
            else if (Input.GetKeyDown(KeyCode.Keypad7))
                MarkPermutationsFor(7);
            else if (Input.GetKeyDown(KeyCode.Keypad8))
                MarkPermutationsFor(8);
            else if (Input.GetKeyDown(KeyCode.Keypad9))
                MarkPermutationsFor(9);
        }

        /// <summary>
        /// Marks all permutations that contain the given number.
        /// </summary>
        /// <param name="number">The number to mark all permutations for.</param>
        private void MarkPermutationsFor(int number)
        {
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 8; y++)
                    for (int z = 0; z < 7; z++) {
                        _permutations[x, y, z].Uncheck();

                        if (SudokuData.Permutations[x, y, z, 0] == number ||
                            SudokuData.Permutations[x, y, z, 1] == number ||
                            SudokuData.Permutations[x, y, z, 2] == number)
                            _permutations[x, y, z].Check(true, number);
                    }
        }
    }
}