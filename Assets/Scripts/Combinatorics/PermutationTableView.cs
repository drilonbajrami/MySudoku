using UnityEngine;
using System;

/// <summary>
/// Visualizes all the possible three number permutations of range from 1 to 9, in a readable table format.
/// </summary>
public class PermutationTableView : MonoBehaviour
{
    [SerializeField] private Permutation _permutationPrefab;
    private Permutation[,,] _permutations = new Permutation[9, 8, 7];

    private Permutation[] _combinations = new Permutation[84];

    /// <summary>
    /// Spawns the permutation's table.
    /// </summary>
    private void Awake()
    {
        SpawnPermutations();
        SpawnCombinations();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad9))
        {
            foreach (Permutation perm in _permutations)
                perm.gameObject.SetActive(!perm.gameObject.activeSelf);

            foreach (Permutation perm in _combinations)
                perm.gameObject.SetActive(!perm.gameObject.activeSelf);
        }
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

                    permutation.SetDigits(new int[] { SudokuData.Permutations[x, y, z, 0],
                                                      SudokuData.Permutations[x, y, z, 1],
                                                      SudokuData.Permutations[x, y, z, 2] });
                    _permutations[x, y, z] = permutation;
                }
            }
        }
    }

    private void SpawnCombinations()
    {
        float l = _permutationPrefab.RectTransform.sizeDelta.x + 3;
        float h = _permutationPrefab.RectTransform.sizeDelta.y + 3;

        for(int y = 0; y < 12; y++)
        {
            for(int x = 0; x < 7; x++)
            {
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
    public bool CheckPermutation(int[] permutation, bool horizontal, int box)
    {
        int[] pIndex = SudokuData.FindPermutationIndex(permutation);

        int[] comb = new int[3];
        permutation.CopyTo(comb, 0);
        Array.Sort(comb);
        for(int i = SudokuData.CombinationGroups[comb[0] - 1, 0]; i < SudokuData.CombinationGroups[comb[0] - 1, 1] + 1; i++)
            if (comb[1] == SudokuData.Combinations[i, 1] && comb[2] == SudokuData.Combinations[i, 2])
                _combinations[i].Check(horizontal, box);

        return _permutations[pIndex[0], pIndex[1], pIndex[2]].Check(horizontal, box);
    }

    /// <summary>
    /// Unchecks/unmarks all permutations.
    /// </summary>
    public void UncheckPermutations()
    {
        for(int x = 0; x < 9; x++)
            for(int y = 0; y < 8; y++)
                for (int z = 0; z < 7; z++)
                    _permutations[x, y, z].Uncheck();

        for(int i = 0; i < _combinations.Length; i++)
            _combinations[i].Uncheck();
    }
}