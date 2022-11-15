using System.Collections.Generic;

namespace MySudoku
{
    /// <summary>
    /// Sudoku solving techniques.
    /// </summary>
    public enum Technique
    {
        NakedSingle    = 0,
        HiddenSingle   = 1,
        CandidateLines = 2,
        MultipleLines  = 3,
        NakedPairs     = 4,
        HiddenPairs    = 5,
        NakedTriples   = 6,
        HiddenTriples  = 7,
        XWing          = 8,
        YWing          = 9,
        ForcingChains  = 10,
        NakedQuads     = 11,
        HiddenQuads    = 12,
        Swordfish      = 13
    }

    /// <summary>
    /// Sudoku puzzle difficulties.
    /// </summary>
    public enum Difficulty
    {
        Beginner,
        Easy,
        Medium,
        Tricky,
        Fiendish,
        Diabolical
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
            { Difficulty.Beginner,   (3600, 4500)   },
            { Difficulty.Easy,       (4300, 5500)   },
            { Difficulty.Medium,     (5300, 6900)   },
            { Difficulty.Tricky,     (6500, 9300)   },
            { Difficulty.Fiendish,   (8300, 14000)  },
            { Difficulty.Diabolical, (11000, 25000) }
        };

        /// <summary>
        /// List of available sudoku techniques to use.
        /// </summary>
        public static List<ISudokuTechnique> techniques = new() {
            new NakedSingle(),
            new HiddenSingle(),
            new CandidateLines(),
            new MultipleLines(),
            new NakedPairs(),
            new HiddenPairs()
        };

        /// <summary>
        /// Tries to apply one of the available sudoku techniques for this sudoku puzzle.
        /// </summary>
        /// <param name="sudoku">The sudoku puzzle.</param>
        /// <param name="notes">The notes for the sudoku puzzle.</param>
        /// <returns>Whether it can apply one of the available sudoku techniques or not.</returns>
        public static bool ApplyTechniques(this int[,] sudoku, bool[,] notes)
        {
            for (int i = 0; i < techniques.Count; i++)
                if (techniques[i].ApplyTechnique(sudoku, notes)) {
                    SudokuGenerator.techniquesUsed[i]++;
                    return true; }
            return false;
        }

        /// <summary>
        /// Toggle the console log for all sudoku techniques.
        /// </summary>
        public static void ToggleConsoleLog()
        {
            for (int i = 0; i < techniques.Count; i++)
                techniques[i].LogConsole = !techniques[i].LogConsole;
        }

        /// <summary>
        /// Resets the usage count for all available sudou techniques.
        /// </summary>
        public static void ResetTechniqueUsageCount()
        {
            for (int i = 0; i < techniques.Count; i++)
                techniques[i].TimesUsed = 0;
        }
    }
}