using UnityEngine;

/// <summary>
/// Data class for sudoku number combinations and permutations.
/// </summary>
public static class SudokuData
{
    /// <summary>
    /// The number of unique sudoku puzzles.
    /// </summary>
    public static string SudokuPuzzlesCount = "6,670,903,752,021,072,936,960";

    /// <summary>
    /// All possible three number combinations from 1 to 9.
    /// </summary>
    public static int[,] Combinations = new int[84, 3] {
        // Combinations including and starting with '1'
        {1, 2, 3}, {1, 2, 4}, {1, 2, 5}, {1, 2, 6}, {1, 2, 7}, {1, 2, 8}, {1, 2, 9},
        {1, 3, 4}, {1, 3, 5}, {1, 3, 6}, {1, 3, 7}, {1, 3, 8}, {1, 3, 9},
        {1, 4, 5}, {1, 4, 6}, {1, 4, 7}, {1, 4, 8}, {1, 4, 9},
        {1, 5, 6}, {1, 5, 7}, {1, 5, 8}, {1, 5, 9},
        {1, 6, 7}, {1, 6, 8}, {1, 6, 9},
        {1, 7, 8}, {1, 7, 9},
        {1, 8, 9},
        
        // Combinations including and starting with '2'
        {2, 3, 4}, {2, 3, 5}, {2, 3, 6}, {2, 3, 7}, {2, 3, 8}, {2, 3, 9},
        {2, 4, 5}, {2, 4, 6}, {2, 4, 7}, {2, 4, 8}, {2, 4, 9},
        {2, 5, 6}, {2, 5, 7}, {2, 5, 8}, {2, 5, 9},
        {2, 6, 7}, {2, 6, 8}, {2, 6, 9},
        {2, 7, 8}, {2, 7, 9},
        {2, 8, 9},

        // Combinations including and starting with '3'
        {3, 4, 5}, {3, 4, 6}, {3, 4, 7}, {3, 4, 8}, {3, 4, 9},
        {3, 5, 6}, {3, 5, 7}, {3, 5, 8}, {3, 5, 9},
        {3, 6, 7}, {3, 6, 8}, {3, 6, 9},
        {3, 7, 8}, {3, 7, 9},
        {3, 8, 9},

        // Combinations including and starting with '4'
        {4, 5, 6}, {4, 5, 7}, {4, 5, 8}, {4, 5, 9},
        {4, 6, 7}, {4, 6, 8}, {4, 6, 9},
        {4, 7, 8}, {4, 7, 9},
        {4, 8, 9}, 
        
        // Combinations including and starting with '5'
        {5, 6, 7}, {5, 6, 8}, {5, 6, 9},
        {5, 7, 8}, {5, 7, 9},
        {5, 8, 9}, 
        
        // Combinations including and starting with '6'
        {6, 7, 8}, {6, 7, 9},
        {6, 8, 9},

        // Combinations including and starting with '7'
        {7, 8, 9}
    };

    /// <summary>
    /// Combination groups by number. 1 (0) -> 28 combinations, 2 (1) -> 21 combintations,... till 7 (6).<br/>
    /// Faster searching by eliminating groups, the lower the number present in the combination is,<br/>
    /// the faster searching for possible combinations from the Combination array.
    /// </summary>
    public static int[,] CombinationGroups = new int[,] {
        { 0, 27 },
        { 28, 48 },
        { 49, 63 },
        { 64, 73 },
        { 74, 79 },
        { 80, 82 },
        { 83, 83 }
    };

    /// <summary>
    /// All possible three number permutations of the range from 1 to 9.<br/>
    /// 504 permutations in total, split in 9 main groups for each starting number.<br/>
    /// Each main group contains 8 subgroups where each subgroup contains 7 different permutations.<br/>
    /// Indexing: [x -> Main group, y -> Subgroup, z -> Permutation (7 on each subgroup 'y')]<br/>
    /// 9 * 8 * 7 = 504.
    /// </summary>
    public static int[,,,] Permutations = new int[9, 8, 7, 3] {
        // Permutations starting with 1
        {
            { {1, 2, 3}, {1, 2, 4}, {1, 2, 5}, {1, 2, 6}, {1, 2, 7}, {1, 2, 8}, {1, 2, 9} },
            { {1, 3, 2}, {1, 3, 4}, {1, 3, 5}, {1, 3, 6}, {1, 3, 7}, {1, 3, 8}, {1, 3, 9} },
            { {1, 4, 2}, {1, 4, 3}, {1, 4, 5}, {1, 4, 6}, {1, 4, 7}, {1, 4, 8}, {1, 4, 9} },
            { {1, 5, 2}, {1, 5, 3}, {1, 5, 4}, {1, 5, 6}, {1, 5, 7}, {1, 5, 8}, {1, 5, 9} },
            { {1, 6, 2}, {1, 6, 3}, {1, 6, 4}, {1, 6, 5}, {1, 6, 7}, {1, 6, 8}, {1, 6, 9} },
            { {1, 7, 2}, {1, 7, 3}, {1, 7, 4}, {1, 7, 5}, {1, 7, 6}, {1, 7, 8}, {1, 7, 9} },
            { {1, 8, 2}, {1, 8, 3}, {1, 8, 4}, {1, 8, 5}, {1, 8, 6}, {1, 8, 7}, {1, 8, 9} },
            { {1, 9, 2}, {1, 9, 3}, {1, 9, 4}, {1, 9, 5}, {1, 9, 6}, {1, 9, 7}, {1, 9, 8} }
        },
        // Permutations starting with 2
        {
            { {2, 1, 3}, {2, 1, 4}, {2, 1, 5}, {2, 1, 6}, {2, 1, 7}, {2, 1, 8}, {2, 1, 9} },
            { {2, 3, 1}, {2, 3, 4}, {2, 3, 5}, {2, 3, 6}, {2, 3, 7}, {2, 3, 8}, {2, 3, 9} },
            { {2, 4, 1}, {2, 4, 3}, {2, 4, 5}, {2, 4, 6}, {2, 4, 7}, {2, 4, 8}, {2, 4, 9} },
            { {2, 5, 1}, {2, 5, 3}, {2, 5, 4}, {2, 5, 6}, {2, 5, 7}, {2, 5, 8}, {2, 5, 9} },
            { {2, 6, 1}, {2, 6, 3}, {2, 6, 4}, {2, 6, 5}, {2, 6, 7}, {2, 6, 8}, {2, 6, 9} },
            { {2, 7, 1}, {2, 7, 3}, {2, 7, 4}, {2, 7, 5}, {2, 7, 6}, {2, 7, 8}, {2, 7, 9} },
            { {2, 8, 1}, {2, 8, 3}, {2, 8, 4}, {2, 8, 5}, {2, 8, 6}, {2, 8, 7}, {2, 8, 9} },
            { {2, 9, 1}, {2, 9, 3}, {2, 9, 4}, {2, 9, 5}, {2, 9, 6}, {2, 9, 7}, {2, 9, 8} }
        },
        // Permutations starting with 3
        {
            { {3, 1, 2}, {3, 1, 4}, {3, 1, 5}, {3, 1, 6}, {3, 1, 7}, {3, 1, 8}, {3, 1, 9} },
            { {3, 2, 1}, {3, 2, 4}, {3, 2, 5}, {3, 2, 6}, {3, 2, 7}, {3, 2, 8}, {3, 2, 9} },
            { {3, 4, 1}, {3, 4, 2}, {3, 4, 5}, {3, 4, 6}, {3, 4, 7}, {3, 4, 8}, {3, 4, 9} },
            { {3, 5, 1}, {3, 5, 2}, {3, 5, 4}, {3, 5, 6}, {3, 5, 7}, {3, 5, 8}, {3, 5, 9} },
            { {3, 6, 1}, {3, 6, 2}, {3, 6, 4}, {3, 6, 5}, {3, 6, 7}, {3, 6, 8}, {3, 6, 9} },
            { {3, 7, 1}, {3, 7, 2}, {3, 7, 4}, {3, 7, 5}, {3, 7, 6}, {3, 7, 8}, {3, 7, 9} },
            { {3, 8, 1}, {3, 8, 2}, {3, 8, 4}, {3, 8, 5}, {3, 8, 6}, {3, 8, 7}, {3, 8, 9} },
            { {3, 9, 1}, {3, 9, 2}, {3, 9, 4}, {3, 9, 5}, {3, 9, 6}, {3, 9, 7}, {3, 9, 8} }
        },
        // Permutations starting with 4
        {
            { {4, 1, 2}, {4, 1, 3}, {4, 1, 5}, {4, 1, 6}, {4, 1, 7}, {4, 1, 8}, {4, 1, 9} },
            { {4, 2, 1}, {4, 2, 3}, {4, 2, 5}, {4, 2, 6}, {4, 2, 7}, {4, 2, 8}, {4, 2, 9} },
            { {4, 3, 1}, {4, 3, 2}, {4, 3, 5}, {4, 3, 6}, {4, 3, 7}, {4, 3, 8}, {4, 3, 9} },
            { {4, 5, 1}, {4, 5, 2}, {4, 5, 3}, {4, 5, 6}, {4, 5, 7}, {4, 5, 8}, {4, 5, 9} },
            { {4, 6, 1}, {4, 6, 2}, {4, 6, 3}, {4, 6, 5}, {4, 6, 7}, {4, 6, 8}, {4, 6, 9} },
            { {4, 7, 1}, {4, 7, 2}, {4, 7, 3}, {4, 7, 5}, {4, 7, 6}, {4, 7, 8}, {4, 7, 9} },
            { {4, 8, 1}, {4, 8, 2}, {4, 8, 3}, {4, 8, 5}, {4, 8, 6}, {4, 8, 7}, {4, 8, 9} },
            { {4, 9, 1}, {4, 9, 2}, {4, 9, 3}, {4, 9, 5}, {4, 9, 6}, {4, 9, 7}, {4, 9, 8} }
        },
        // Permutations starting with 5
        {
            { {5, 1, 2}, {5, 1, 3}, {5, 1, 4}, {5, 1, 6}, {5, 1, 7}, {5, 1, 8}, {5, 1, 9} },
            { {5, 2, 1}, {5, 2, 3}, {5, 2, 4}, {5, 2, 6}, {5, 2, 7}, {5, 2, 8}, {5, 2, 9} },
            { {5, 3, 1}, {5, 3, 2}, {5, 3, 4}, {5, 3, 6}, {5, 3, 7}, {5, 3, 8}, {5, 3, 9} },
            { {5, 4, 1}, {5, 4, 2}, {5, 4, 3}, {5, 4, 6}, {5, 4, 7}, {5, 4, 8}, {5, 4, 9} },
            { {5, 6, 1}, {5, 6, 2}, {5, 6, 3}, {5, 6, 4}, {5, 6, 7}, {5, 6, 8}, {5, 6, 9} },
            { {5, 7, 1}, {5, 7, 2}, {5, 7, 3}, {5, 7, 4}, {5, 7, 6}, {5, 7, 8}, {5, 7, 9} },
            { {5, 8, 1}, {5, 8, 2}, {5, 8, 3}, {5, 8, 4}, {5, 8, 6}, {5, 8, 7}, {5, 8, 9} },
            { {5, 9, 1}, {5, 9, 2}, {5, 9, 3}, {5, 9, 4}, {5, 9, 6}, {5, 9, 7}, {5, 9, 8} }
        },
        // Permutations starting with 6
        {
            { {6, 1, 2}, {6, 1, 3}, {6, 1, 4}, {6, 1, 5}, {6, 1, 7}, {6, 1, 8}, {6, 1, 9} },
            { {6, 2, 1}, {6, 2, 3}, {6, 2, 4}, {6, 2, 5}, {6, 2, 7}, {6, 2, 8}, {6, 2, 9} },
            { {6, 3, 1}, {6, 3, 2}, {6, 3, 4}, {6, 3, 5}, {6, 3, 7}, {6, 3, 8}, {6, 3, 9} },
            { {6, 4, 1}, {6, 4, 2}, {6, 4, 3}, {6, 4, 5}, {6, 4, 7}, {6, 4, 8}, {6, 4, 9} },
            { {6, 5, 1}, {6, 5, 2}, {6, 5, 3}, {6, 5, 4}, {6, 5, 7}, {6, 5, 8}, {6, 5, 9} },
            { {6, 7, 1}, {6, 7, 2}, {6, 7, 3}, {6, 7, 4}, {6, 7, 5}, {6, 7, 8}, {6, 7, 9} },
            { {6, 8, 1}, {6, 8, 2}, {6, 8, 3}, {6, 8, 4}, {6, 8, 5}, {6, 8, 7}, {6, 8, 9} },
            { {6, 9, 1}, {6, 9, 2}, {6, 9, 3}, {6, 9, 4}, {6, 9, 5}, {6, 9, 7}, {6, 9, 8} }
        },
        // Permutations starting with 7
        {
            { {7, 1, 2}, {7, 1, 3}, {7, 1, 4}, {7, 1, 5}, {7, 1, 6}, {7, 1, 8}, {7, 1, 9} },
            { {7, 2, 1}, {7, 2, 3}, {7, 2, 4}, {7, 2, 5}, {7, 2, 6}, {7, 2, 8}, {7, 2, 9} },
            { {7, 3, 1}, {7, 3, 2}, {7, 3, 4}, {7, 3, 5}, {7, 3, 6}, {7, 3, 8}, {7, 3, 9} },
            { {7, 4, 1}, {7, 4, 2}, {7, 4, 3}, {7, 4, 5}, {7, 4, 6}, {7, 4, 8}, {7, 4, 9} },
            { {7, 5, 1}, {7, 5, 2}, {7, 5, 3}, {7, 5, 4}, {7, 5, 6}, {7, 5, 8}, {7, 5, 9} },
            { {7, 6, 1}, {7, 6, 2}, {7, 6, 3}, {7, 6, 4}, {7, 6, 5}, {7, 6, 8}, {7, 6, 9} },
            { {7, 8, 1}, {7, 8, 2}, {7, 8, 3}, {7, 8, 4}, {7, 8, 5}, {7, 8, 6}, {7, 8, 9} },
            { {7, 9, 1}, {7, 9, 2}, {7, 9, 3}, {7, 9, 4}, {7, 9, 5}, {7, 9, 6}, {7, 9, 8} }
        },
        // Permutations starting with 8
        {
            { {8, 1, 2}, {8, 1, 3}, {8, 1, 4}, {8, 1, 5}, {8, 1, 6}, {8, 1, 7}, {8, 1, 9} },
            { {8, 2, 1}, {8, 2, 3}, {8, 2, 4}, {8, 2, 5}, {8, 2, 6}, {8, 2, 7}, {8, 2, 9} },
            { {8, 3, 1}, {8, 3, 2}, {8, 3, 4}, {8, 3, 5}, {8, 3, 6}, {8, 3, 7}, {8, 3, 9} },
            { {8, 4, 1}, {8, 4, 2}, {8, 4, 3}, {8, 4, 5}, {8, 4, 6}, {8, 4, 7}, {8, 4, 9} },
            { {8, 5, 1}, {8, 5, 2}, {8, 5, 3}, {8, 5, 4}, {8, 5, 6}, {8, 5, 7}, {8, 5, 9} },
            { {8, 6, 1}, {8, 6, 2}, {8, 6, 3}, {8, 6, 4}, {8, 6, 5}, {8, 6, 7}, {8, 6, 9} },
            { {8, 7, 1}, {8, 7, 2}, {8, 7, 3}, {8, 7, 4}, {8, 7, 5}, {8, 7, 6}, {8, 7, 9} },
            { {8, 9, 1}, {8, 9, 2}, {8, 9, 3}, {8, 9, 4}, {8, 9, 5}, {8, 9, 6}, {8, 9, 7} }
        },
        // Permutations starting with 9
        {
            { {9, 1, 2}, {9, 1, 3}, {9, 1, 4}, {9, 1, 5}, {9, 1, 6}, {9, 1, 7}, {9, 1, 8} },
            { {9, 2, 1}, {9, 2, 3}, {9, 2, 4}, {9, 2, 5}, {9, 2, 6}, {9, 2, 7}, {9, 2, 8} },
            { {9, 3, 1}, {9, 3, 2}, {9, 3, 4}, {9, 3, 5}, {9, 3, 6}, {9, 3, 7}, {9, 3, 8} },
            { {9, 4, 1}, {9, 4, 2}, {9, 4, 3}, {9, 4, 5}, {9, 4, 6}, {9, 4, 7}, {9, 4, 8} },
            { {9, 5, 1}, {9, 5, 2}, {9, 5, 3}, {9, 5, 4}, {9, 5, 6}, {9, 5, 7}, {9, 5, 8} },
            { {9, 6, 1}, {9, 6, 2}, {9, 6, 3}, {9, 6, 4}, {9, 6, 5}, {9, 6, 7}, {9, 6, 8} },
            { {9, 7, 1}, {9, 7, 2}, {9, 7, 3}, {9, 7, 4}, {9, 7, 5}, {9, 7, 6}, {9, 7, 8} },
            { {9, 8, 1}, {9, 8, 2}, {9, 8, 3}, {9, 8, 4}, {9, 8, 5}, {9, 8, 6}, {9, 8, 7} }
        }
    };

    /// <summary>
    /// Returns the indexes of possible permutations from the <see cref="Permutations"/> array.
    /// </summary>
    /// <param name="permutation">The given permutation, can consist of 3 or 6 numbers.</param>
    /// <returns>A 2D array of possible permutation indexes within the <see cref="Permutations"/> array.</returns>
    public static int[,] GetPossiblePermutations(int[] permutation)
    {
        // Depending on the given permutation, if it only consist of 3 numbers then there are 120 possible permutations.
        // If it consist of 6 numbers then there are only 6 possible permutation for whatever combination of those 6 numbers.
        int possiblePermutations = permutation.Length == 3 ? 120 : 6;
        int[,] permutationIndices = new int[possiblePermutations, 3];
        int permutationCount = 0;
        for (int x = 0; x < 9; x++) {
            // Check if permutation does not have number "x + 1" then continue.
            if (!permutation.HasNumber(x + 1)) {
                for (int y = 0; y < 8; y++) {
                    // Check if permutation does not have number "y" then continue.
                    if (!permutation.HasNumber(Permutations[x, y, 0, 1])) {
                        for (int z = 0; z < 7; z++) {
                            // Check if permutation does not have number "z" then continue.
                            if (!permutation.HasNumber(Permutations[x, y, z, 2])) {
                                permutationIndices[permutationCount, 0] = x;
                                permutationIndices[permutationCount, 1] = y;
                                permutationIndices[permutationCount, 2] = z;
                                permutationCount++;
                            }
                        }
                    }
                }
            }
        }

        return permutationIndices;
    }

    /// <summary>
    /// Check if this permutation has the given number.
    /// </summary>
    /// <param name="permutation">This permutation.</param>
    /// <param name="number">The number to check for.</param>
    /// <returns>True if it contains, false if not.</returns>
    private static bool HasNumber(this int[] permutation, int number)
    {
        for (int i = 0; i < permutation.Length; i++)
            if (number == permutation[i]) return true;
        return false;
    }

    /// <summary>
    /// Finds the index of a permutation in the <see cref="Permutations"/> array.
    /// </summary>
    /// <param name="permutation">The permutation to find the index for.</param>
    /// <returns>The index of the given permutation within the <see cref="Permutations"/> array.</returns>
    public static int[] FindPermutationIndex(int[] permutation)
    {
        int[] index = new int[3];
        index[0] = permutation[0] - 1;

        for (int y = Mathf.Clamp(permutation[1] - 2, 0, 7); y < 8; y++)
            if (Permutations[index[0], y, 0, 1] == permutation[1]) {
                index[1] = y;
                break;
            }

        for (int z = Mathf.Clamp(permutation[2] - 3, 0, 6); z < 7; z++)
            if (Permutations[index[0], index[1], z, 2] == permutation[2]) {
                index[2] = z;
                break;
            }

        return index;
    }
}