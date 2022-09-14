using UnityEngine;

/// <summary>
/// Visualizes all the possible three number permutations of range from 1 to 9, in a readable table format.
/// </summary>
public class PermutationTableView : MonoBehaviour
{
    [SerializeField] private Permutation _permutationPrefab;
    private Permutation[,,] _permutations = new Permutation[9, 8, 7];

    /// <summary>
    /// Spawns the permutation's table.
    /// </summary>
    private void Awake() => SpawnPermutations();
    
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
    }
}