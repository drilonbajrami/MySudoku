using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace MySudoku
{
    // Single Candidate only technique sudoku highest score of 5600

    public static class SudokuTechniques
    {
        /// <summary>
        /// Sudoku difficulties score map, with lower and upper ranges.
        /// </summary>
        public static Dictionary<Difficulty, (int lower, int upper)> DifficultyMap = new() {
            { Difficulty.BEGGINER,   (3600, 4500)   },
            { Difficulty.EASY,       (4300, 5500)   },
            { Difficulty.MEDIUM,     (5300, 6900)   },
            { Difficulty.TRICKY,     (6500, 9300)   },
            { Difficulty.FIENDISH,   (8300, 14000)  },
            { Difficulty.DIABOLICAL, (11000, 25000) }
        };

        public static bool NakedSingle(this int[,] sudoku, bool[,] notes)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    if (sudoku[row, col] == 0) {
                        int value = 0;
                        bool singleCandidate = false;
                        for (int i = 0; i < 9; i++) {
                            if (notes[row * 9 + col, i]) {
                                if (singleCandidate) {
                                    value = 0;
                                    break;
                                }
                                else {
                                    singleCandidate = true;
                                    value = i + 1;
                                }
                            }
                        }

                        if (value != 0) {
                            sudoku[row, col] = value;
                            notes.UpdateNotes(sudoku, (row, col), 0, value);
                            return true;
                        }
                    }
                }
                    return false;
        }

        public static bool HiddenSingle(this Sudoku sudoku)
        {
            return false;
        }

        public static bool CandidateLines(this Sudoku sudoku)
        {
            return false;
        }

        public static bool DoublePairs(this Sudoku sudoku)
        {
            return false;
        }

        public static bool MultipleLines(this Sudoku sudoku)
        {
            return false;
        }

        public static bool NakedPairs(this Sudoku sudoku)
        {
            return false;
        }

        public static bool HiddenPairs(this Sudoku sudoku)
        {
            return false;
        }

        public static bool NakedTriples(this Sudoku sudoku)
        {
            return false;
        }

        public static bool HiddenTriples(this Sudoku sudoku)
        {
            return false;
        }

        public static bool XWing(this Sudoku sudoku)
        {
            return false;
        }

        public static bool YWing(this Sudoku sudoku)
        {
            return false;
        }

        public static bool NakedQuads(this Sudoku sudoku)
        {
            return false;
        }

        public static bool HiddenQuads(this Sudoku sudoku)
        {
            return false;
        }

        public static bool Swordfish(this Sudoku sudoku)
        {
            return false;
        }

        /* Sudoku Techniques:
          * 1. Single Candidate - https://www.sudokuoftheday.com/techniques/single-candidate
               Obvious/Naked Singles - https://sudoku.com/sudoku-rules/obvious-singles/

          * 2. Single Position - https://www.sudokuoftheday.com/techniques/single-position
               Hidden Singles - https://sudoku.com/sudoku-rules/hidden-singles/

          * 3. Candidate Lines - https://www.sudokuoftheday.com/techniques/candidate-lines
               Pointing Pairs - https://sudoku.com/sudoku-rules/pointing-pairs/

            4. Double Pairs - https://www.sudokuoftheday.com/techniques/double-pairs

            5. Multiple Lines - https://www.sudokuoftheday.com/techniques/multiple-lines

          * 6. Naked Pairs/Triples - https://www.sudokuoftheday.com/techniques/naked-pairs-triples
               Obivous Pairs - https://sudoku.com/sudoku-rules/obvious-pairs/
               Obvious Triples - https://sudoku.com/sudoku-rules/obvious-triples/

          * 7. Hidden Pairs/Triples - https://www.sudokuoftheday.com/techniques/hidden-pairs-triples
               Hidden Pairs - https://sudoku.com/sudoku-rules/hidden-pairs/
               Hidden Triples - https://sudoku.com/sudoku-rules/hidden-triples/

            7.1. Naked Quads - https://www.sudoku.org.uk/SolvingTechniques/NakedQuads.asp
            7.2. Hidden Quads - https://www.sudoku.org.uk/SolvingTechniques/HiddenTriples.asp

          * 8. X-Wing - https://www.sudokuoftheday.com/techniques/x-wings
               X-Wing - https://sudoku.com/sudoku-rules/h-wing/

            8.1. Y-Wing - https://sudoku.com/sudoku-rules/y-wing/

            9. Swordfish - https://www.sudokuoftheday.com/techniques/swordfish

            10. Forcing Chains - https://www.sudokuoftheday.com/techniques/forcing-chains

            Technique               Cost   Cost for subsequent use
            Easy:
            1. Single Candidate -   100 -> 100
            2. Single Position  -   100 -> 100
            Medium:
            3. Candidate Lines  -   350 -> 200
            4. Double Pairs     -   500 -> 250
            5. Multiple Lines   -   700 -> 400
            Advanced:
            6. Naked Pairs       -   750 -> 500
            7. Hidden Pairs      -   1500 -> 1200
            8. Naked Triples     -   2000 -> 1400
            9. Hidden Triples    -   2400 -> 1600
            Master:
            10. X-Wing          -   2800 -> 1600
           *10. Y-Wing          -   3000 -> 1800
            11. Forcing Chains  -   4200 -> 2100
            12. Naked Quad      -   5000 -> 4000
            13. Hidden Quad     -   7000 -> 5000
            14. Swordfish       -   8000 -> 6000    
         */
        }

    /// <summary>
    /// Sudoku solving techniques.
    /// </summary>
    public enum Techniques
    {
        SINGLE_CANDIDATE,
        SINGLE_POSITION,
        CANDIDATE_LINES,
        DOUBLE_PAIRS,
        MULTIPLE_LINES,
        NAKED_PAIRS,
        HIDDEN_PAIRS,
        NAKED_TRIPLES,
        HIDDEN_TRIPLES,
        X_WING,
        Y_WING,
        FORCING_CHAINS,
        NAKED_QUADS,
        HIDDEN_QUADS,
        SWORDFISH
    }

    /// <summary>
    /// Sudoku puzzle difficulties.
    /// </summary>
    public enum Difficulty
    {
        BEGGINER,
        EASY,
        MEDIUM,
        TRICKY,
        FIENDISH,
        DIABOLICAL
    }
}