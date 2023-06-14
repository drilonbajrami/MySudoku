using System.Collections.Generic;

namespace MySudoku
{
    /// <summary>
    /// Sudoku solving techniques.
    /// </summary>
    public enum Technique
    {
        Naked_Single    = 0,
        Hidden_Single   = 1,
        Candidate_Lines = 2,
        Multiple_Lines  = 3,
        Naked_Pairs     = 4,
        Hidden_Pairs    = 5,
        Naked_Triples   = 6,
        Hidden_Triples  = 7,
        X_Wing          = 8,
        Y_Wing          = 9,
        Forcing_Chains  = 10,
        Naked_Quads     = 11,
        Hidden_Quads    = 12,
        Swordfish       = 13
    }

    /// <summary>
    /// Sudoku puzzle difficulties.
    /// </summary>
    public enum Difficulty
    {
        Beginner,
        Easy,
        Medium,
        Hard,
        Extreme,
        Evil
    }

    /// <summary>
    /// Manager for all sudoku techniques.
    /// </summary>
    public static class SudokuTechniques
    {
        /// <summary>
        /// Sudoku difficulties score map, with lower and upper ranges.
        /// </summary>
        public static Dictionary<Difficulty, (int lower, int upper)> DifficultyMap = new() {
            { Difficulty.Beginner, (3600, 4500)   },
            { Difficulty.Easy,     (4300, 5500)   },
            { Difficulty.Medium,   (5300, 6900)   },
            { Difficulty.Hard,     (6500, 9300)   },
            { Difficulty.Extreme,  (8300, 14000)  },
            { Difficulty.Evil,     (11000, 25000) }
        };

        /// <summary>
        /// List of available sudoku techniques to use.
        /// </summary>
        public static List<ISudokuTechnique> Techniques = new() {
            new NakedSingle(),
            new HiddenSingle(),
            new CandidateLines(),
            new MultipleLines(),
            new NakedPairs(),
            new HiddenPairs(),
            new NakedTriples(),
            new HiddenTriples()
        };

        /// <summary>
        /// Tries to apply one of the available sudoku techniques for this sudoku puzzle.
        /// </summary>
        /// <param name="sudoku">The sudoku puzzle.</param>
        /// <param name="notes">The notes for the sudoku puzzle.</param>
        /// <returns>The cost of the applied technique if possible, otherwise returns 0.</returns>
        public static int ApplyTechniques(this int[,] sudoku, bool[,] notes)
        {
            for (int i = 0; i < Techniques.Count; i++)
                if (Techniques[i].ApplyTechnique(sudoku, notes, out int cost))
                    return cost; 

            return 0;
        }

        /// <summary>
        /// Toggle the console log for all sudoku techniques.
        /// </summary>
        public static void ToggleConsoleLog()
        {
            for (int i = 0; i < Techniques.Count; i++)
                Techniques[i].LogConsole = !Techniques[i].LogConsole;
        }

        /// <summary>
        /// Resets the usage count for all available sudou techniques.
        /// </summary>
        public static void ResetTechniqueUsageCount()
        {
            for (int i = 0; i < Techniques.Count; i++)
                Techniques[i].TimesUsed = 0;
        }
    }
}