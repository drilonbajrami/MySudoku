using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static List<Technique> techniquesUsed = new();

        public static void PrintUsedTechniques()
        {
            if (techniquesUsed.Count != 0) {
                for (int i = 0; i < techniquesUsed.Count; i++)
                    Debug.Log($"{i}. {Enum.GetName(typeof(Technique), techniquesUsed[i])}");
            }
        }

        public static void ClearUsedTechniques() => techniquesUsed.Clear();

        public static bool ApplyTechniques(this int[,] sudoku, bool[,] notes)
        {
            int numberOfTechniques = Enum.GetNames(typeof(Technique)).Length;
            bool applicable = false;

            for (int i = 0; i < numberOfTechniques; i++) {
                applicable = TryTechnique(sudoku, notes, i);
                if (applicable) {
                    //techniquesUsed.Add((Technique)i);
                    return true;
                }
            }

            return applicable;
        }

        private static bool TryTechnique(int[,] sudoku, bool[,] notes, int techniqueRank)
        {
            bool applicable;
            switch (techniqueRank) {
                case 0:
                    applicable = sudoku.NakedSingle(notes);
                    break;
                case 1:
                    applicable = sudoku.HiddenSingle(notes);
                    break;
                case 2:
                    applicable = sudoku.CandidateLines(notes);
                    break;
                case 3:
                    applicable = sudoku.DoublePairs(notes);
                    break;
                case 4:
                    applicable = sudoku.MultipleLines(notes);
                    break;
                case 5:
                    applicable = sudoku.NakedPair(notes);
                    break;
                case 6:
                    applicable = sudoku.HiddenPair(notes);
                    break;
                case 7:
                    applicable = sudoku.NakedTriple(notes);
                    break;
                case 8:
                    applicable = sudoku.HiddenTriple(notes);
                    break;
                case 9:
                    applicable = sudoku.XWing(notes);
                    break;
                case 10:
                    applicable = sudoku.YWing(notes);
                    break;
                case 11:
                    applicable = sudoku.ForcingChains(notes);
                    break;
                case 12:
                    applicable = sudoku.NakedQuad(notes);
                    break;
                case 13:
                    applicable = sudoku.HiddenQuad(notes);
                    break;
                case 14:
                    applicable = sudoku.Swordfish(notes);
                    break;
                default:
                    applicable = false;
                    break;
            }

            return applicable;
        }

        private static bool NakedSingle(this int[,] sudoku, bool[,] notes)
        {
            // Search for an empty cell.
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Cache the single candidate value and flag.
                    int value = 0;
                    bool singleCandidate = false;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        if (!notes[row * 9 + col, i]) continue;

                        if (singleCandidate) {
                            value = 0;
                            break;
                        }
                        else {
                            singleCandidate = true;
                            value = i + 1;
                        }
                    }

                    // Replace with candidate value if there is only one.
                    if (value != 0) {
                        sudoku[row, col] = value;
                        notes.UpdateNotes(sudoku, (row, col), 0, value);
                        return true;
                    }
                }

            return false;
        }

        private static bool HiddenSingle(this int[,] sudoku, bool[,] notes)
        {
            // Search for an empty cell.
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        // First check if it is a hidden single within the box/region of the current cell.
                        // If not, then proceed with checking the row and column it is on.
                        if (!notes[row * 9 + col, i]) continue;

                        bool isHiddenSingle;

                        // Box
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - row % 3 + j / 3;
                            int nCol = col - col % 3 + j % 3;
                            bool isNotSelf = row != nRow || col != nCol;
                            if (!(isNotSelf && sudoku[nRow, nCol] == 0)) continue;

                            isHiddenSingle = !notes[nRow * 9 + nCol, i];
                            if (!isHiddenSingle) break;
                            else if (j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.UpdateNotes(sudoku, (row, col), 0, i + 1);
                                return true;
                            }
                        }

                        // Row
                        for (int j = 0; j < 9; j++) {
                            if (!(row != j && sudoku[j, col] == 0)) continue;

                            isHiddenSingle = !(row != j && notes[j * 9 + col, i]);
                            if (!isHiddenSingle) break;
                            else if (j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.UpdateNotes(sudoku, (row, col), 0, i + 1);
                                return true;
                            }
                        }

                        // Col
                        for (int j = 0; j < 9; j++) {
                            if (!(col != j && sudoku[row, j] == 0)) continue;

                            isHiddenSingle = !notes[row * 9 + j, i];
                            if (!isHiddenSingle) break;
                            else if (j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.UpdateNotes(sudoku, (row, col), 0, i + 1);
                                return true;
                            }
                        }
                    }
                }

            return false;
        }

        private static bool CandidateLines(this int[,] sudoku, bool[,] notes)
        {
            // Search for an empty cell.
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Process each active note of that cell.
                    for (int i = 0; i < 9; i++) {
                        // If note is not active then skip to next one.
                        if (!notes[row * 9 + col, i]) continue;

                        bool inLineRow = false;
                        bool inLineCol = false;

                        // Check if the current note appears only within a row or column line within the box of cells.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - row % 3 + j / 3;
                            int nCol = col - col % 3 + j % 3;
                            bool isNotSelf = row != nRow || col != nCol;

                            // If not the same cell 
                            if (!(isNotSelf && sudoku[nRow, nCol] == 0)) continue;

                            // Can't be applied if same note appears on a different column and row.
                            if (row != nRow && col != nCol && notes[nRow * 9 + nCol, i]) {
                                inLineRow = inLineCol = false;
                                break; 
                            }
                            else if (row == nRow && col != nCol && !inLineRow)
                                inLineRow = true;
                            else if (row != nRow && col == nCol && !inLineCol)
                                inLineCol = true;
                   
                            if (inLineRow && inLineCol) break;
                        }

                        if (inLineRow) {
                            bool notePresent = false;
                            int currentBoxRow = row - row % 3;
                            for (int j = 0; j < 9; j++)
                                if ((j < currentBoxRow || j > currentBoxRow + 2) && sudoku[j, col] == 0 && notes[j * 9 + col, i]) {
                                    notes[j * 9 + col, i] = false;
                                    notePresent = true;
                                }

                            if (notePresent) return true;
                            else break;
                        }
                        else if (inLineCol) {
                            bool notePresent = false;
                            int currentBoxCol = col - col % 3;
                            for (int j = 0; j < 9; j++)
                                if ((j < currentBoxCol || j > currentBoxCol + 2) && sudoku[row, j] == 0 && notes[row * 9 + j, i]) {
                                    notes[row * 9 + j, i] = false;
                                    notePresent = true;
                                }

                            if (notePresent) return true;
                            else break;
                        }
                    }
                }

            return false;
        }

        private static bool DoublePairs(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool MultipleLines(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool NakedPair(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool HiddenPair(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool NakedTriple(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool HiddenTriple(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool XWing(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool YWing(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool ForcingChains(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool NakedQuad(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool HiddenQuad(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        private static bool Swordfish(this int[,] sudoku, bool[,] notes)
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
            1. Single Candidate(Naked Single) -   100 -> 100
            2. Single Position(Hidden Single)  -   100 -> 100
            Medium:
            3. Candidate Lines  -   350 -> 200
            4. Double Pairs     -   500 -> 250
            5. Multiple Lines   -   700 -> 400
            Advanced:
            6. Naked Pair       -   750 -> 500
            7. Hidden Pair      -   1500 -> 1200
            8. Naked Triple     -   2000 -> 1400
            9. Hidden Triple    -   2400 -> 1600
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
    public enum Technique
    {
        NAKED_SINGLE = 0,
        HIDDEN_SINGLE = 1,
        CANDIDATE_LINES = 2,
        DOUBLE_PAIRS = 3,
        MULTIPLE_LINES = 4,
        NAKED_PAIR = 5,
        HIDDEN_PAIR = 6,
        NAKED_TRIPLE = 7,
        HIDDEN_TRIPLE = 8,
        X_WING = 9,
        Y_WING = 10,
        FORCING_CHAINS = 11,
        NAKED_QUAD = 12,
        HIDDEN_QUAD = 13,
        SWORDFISH = 14
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