using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermutationTableView : MonoBehaviour
{
    [SerializeField] private Permutation _permutationPrefab;

    private Permutation[,,] _permutations = new Permutation[9, 8, 7];

    private void Start()
    {
        SpawnPermutations();
    }

    private void SpawnPermutations()
    {
        float l = _permutationPrefab.RectTransform.sizeDelta.x + 3;
        float h = _permutationPrefab.RectTransform.sizeDelta.y + 3;

        for (int x = 0; x < 9; x++) {
            for (int y = 0; y < 8; y++) {
                for (int z = 0; z < 7; z++) {
                    Permutation permutation = Instantiate(_permutationPrefab, transform);
                    permutation.RectTransform.anchoredPosition = new Vector3(z * l + (x / 4) * (8 * l),
                                                                            (x % 4 * 9 + y) * -h, 0);

                    permutation.SetDigits(new int[] { SudokuData.Permutations[x, y, z, 0],
                                                      SudokuData.Permutations[x, y, z, 1],
                                                      SudokuData.Permutations[x, y, z, 2] });
                    _permutations[x, y, z] = permutation;
                }
            }
        }
    }

    public void CheckPermutation(int[] permutation)
    {
        int[] pIndex = SudokuData.FindPermutationIndex(permutation);
        _permutations[pIndex[0], pIndex[1], pIndex[2]].Check();
    }

    public void UncheckPermutations()
    {
        for(int x = 0; x < 9; x++)
            for(int y = 0; y < 8; y++)
                for (int z = 0; z < 7; z++)
                    _permutations[x, y, z].Uncheck();
    }
}