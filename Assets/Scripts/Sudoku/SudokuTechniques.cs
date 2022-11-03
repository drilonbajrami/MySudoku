using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public static bool NakedSingle(this int[,] sudoku, bool[,] notes)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Set single candidate flag to false.
                    int candidate = 0;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        // If note is inactive then continue with the next note.
                        if (!notes[row * 9 + col, i]) continue;

                        // If there was already a candidate before, then skip this cell.
                        if (candidate != 0) {
                            candidate = 0;
                            break;
                        }
                        else candidate = i + 1;
                    }

                    // If there was only one candidate then use its value.
                    if (candidate != 0) {
                        sudoku[row, col] = candidate;
                        notes.UpdateNotes(sudoku, (row, col), 0, candidate);
                        return true;
                    }
                }

            return false;
        }

        public static bool HiddenSingle(this int[,] sudoku, bool[,] notes)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    // Process empty cells only.
                    if (sudoku[row, col] != 0) continue;

                    // Check each note of the cell.
                    for (int i = 0; i < 9; i++) {
                        if (!notes[row * 9 + col, i]) continue;

                        // Track if the current candidate (i) is a hidden single.
                        bool isHiddenSingle;
                        bool hiddenInBox = true;
                        bool hiddenInRow = true;
                        bool hiddenInCol = true;

                        // First check if it is a hidden single within the box/region, row or column of the current cell.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - (row % 3) + (j / 3); // neighbor box row.
                            int nCol = col - (col % 3) + (j % 3); // neighbor box column.             

                            bool isBoxNeighbor = row != nRow || col != nCol;
                            bool isRowNeighbor = col != j;
                            bool isColNeighbor = row != j;

                            // Box, row and column
                            if (isBoxNeighbor && sudoku[nRow, nCol] == 0 && hiddenInBox) hiddenInBox = !notes[nRow * 9 + nCol, i];
                            if (isRowNeighbor && sudoku[row, j] == 0 && hiddenInRow) hiddenInRow = !notes[row * 9 + j, i];
                            if (isColNeighbor && sudoku[j, col] == 0 && hiddenInCol) hiddenInCol = !notes[j * 9 + col, i];

                            isHiddenSingle = hiddenInBox || hiddenInRow || hiddenInCol;

                            // Check if the current note is still a hidden single when processing all neighbor cells.
                            if (isHiddenSingle && j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.UpdateNotes(sudoku, (row, col), 0, i + 1);
                                return true;
                            }
                            else if (!isHiddenSingle) break; // Skip for current note if it is not a hidden single.
                        }
                    }
                }

            return false;
        }

        public static bool CandidateLines(this int[,] sudoku, bool[,] notes)
        {
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

                        // Check if the current note appears only within a row or column within this box.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - (row % 3) + (j / 3); // neighbor box row.
                            int nCol = col - (col % 3) + (j % 3); // neighbor box column.  
                            bool isNeighbor = row != nRow || col != nCol;

                            // If not the same cell 
                            if (!(isNeighbor && sudoku[nRow, nCol] == 0)) continue;

                            // Can't be applied if same note appears on a different column and row within this box/region.
                            if (row != nRow && col != nCol && notes[nRow * 9 + nCol, i])
                                inLineRow = inLineCol = true;

                            if (inLineRow && inLineCol) break;
                            else if (!inLineRow && row == nRow && col != nCol) inLineRow = notes[nRow * 9 + nCol, i];
                            else if (!inLineCol && row != nRow && col == nCol) inLineCol = notes[nRow * 9 + nCol, i];
                        }

                        bool techniqueIsApplied = false;

                        // If candidate has appared more than once within a row in this box, then check for neighbor cells on the same row in other boxes.
                        if (inLineRow && !inLineCol) {
                            int currentBoxCol = col - col % 3;
                            for (int j = 0; j < 9; j++)
                                // If there is any occurrence of this candidate within row neighbour cells outside of this box then remove it.
                                if ((j < currentBoxCol || j > currentBoxCol + 2) && sudoku[row, j] == 0 && notes[row * 9 + j, i]) {
                                    notes[row * 9 + j, i] = false;
                                    techniqueIsApplied = true;
                                }
                        }
                        // If candidate has appared more than once within a column in this box, then check for neighbor cells on the same column in other boxes.
                        else if (inLineCol && !inLineRow) {
                            int currentBoxRow = row - row % 3;
                            for (int j = 0; j < 9; j++)
                                // If there is any occurrence of this candidate within column neighbour cells outside of this box then remove it.
                                if ((j < currentBoxRow || j > currentBoxRow + 2) && sudoku[j, col] == 0 && notes[j * 9 + col, i]) {
                                    notes[j * 9 + col, i] = false;
                                    techniqueIsApplied = true;
                                }
                        }

                        if (techniqueIsApplied) return true;               
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
            //for (int row = 0; row < 9; row++)
            //    for (int col = 0; col < 9; col++) {
            //        // Process empty cells only.
            //        if (sudoku[row, col] != 0) continue;

            //        // Process each active note of that cell.
            //        for (int i = 0; i < 9; i++) {
            //            // If note is not active then skip to next one.
            //            if (!notes[row * 9 + col, i]) continue;

            //            // Track the flags of whether the current note appears both in a row and column.
            //            bool inBox = false;
            //            bool inBoxRow = false;
            //            bool inBoxCol = false;
            //            bool inRow = false;
            //            bool inCol = false;

            //            // Check if the current note appears on a neighbor cell within a row, column or box.
            //            for (int j = 0; j < 9; j++) {
            //                int boxRow = row - row % 3; // current box starting row (top left cell)
            //                int boxCol = col - col % 3; // current box starting col (top left cell)
            //                int nRow = boxRow + j / 3;  // box neighbor row
            //                int nCol = boxCol + j % 3;  // box neighbor column

            //                // Box
            //                if ((row != nRow || col != nCol) && sudoku[nRow, nCol] == 0 && notes[nRow * 9 + nCol, i]) {
            //                    if (!inBoxRow && row == nRow) inBoxRow = true;
            //                    if (!inBoxCol && col == nCol) inBoxCol = true;
            //                    if (!inBox && row != nRow && col != nCol) inBox = true;
            //                }

            //                // Row (j : col)
            //                if (!inRow && (j < boxCol || j > boxCol + 2) && sudoku[row, j] == 0 && notes[row * 9 + j, i])
            //                    inRow = true;

            //                // Column (j : row)
            //                if (!inCol && (j < boxRow || j > boxRow + 2) && sudoku[j, col] == 0 && notes[j * 9 + col, i])
            //                    inCol = true;

            //                if (inBox && inBoxRow && inBoxCol && inRow && inCol) break;
            //            }

            //            if (inBox && inBoxRow && inBoxCol && inRow && inCol) continue;

            //            if (!inRow && inBoxRow && (!inCol && (inBoxCol || inBox))) {
            //                for (int j = 0; j < 9; j++) {
            //                    int boxRow = row - row % 3; // current box starting row (top left cell)
            //                    int boxCol = col - col % 3; // current box starting col (top left cell)
            //                }

            //            }

            //            // case 1 : inRow(false) and inBoxRow(true) and (inCol(false) 

            //            //bool notePresent = false;
            //            //if (inLineRow) {
            //            //    int currentBoxRow = row - row % 3;
            //            //    for (int j = 0; j < 9; j++)
            //            //        if ((j < currentBoxRow || j > currentBoxRow + 2) && sudoku[j, col] == 0 && notes[j * 9 + col, i]) {
            //            //            notes[j * 9 + col, i] = false;
            //            //            notePresent = true;
            //            //        }
            //            //} else if (inLineCol) {
            //            //    int currentBoxCol = col - col % 3;
            //            //    for (int j = 0; j < 9; j++)
            //            //        if ((j < currentBoxCol || j > currentBoxCol + 2) && sudoku[row, j] == 0 && notes[row * 9 + j, i]) {
            //            //            notes[row * 9 + j, i] = false;
            //            //            notePresent = true;
            //            //        }
            //            //}

            //            //if (notePresent) return true;
            //        }
            //    }

            //return false;
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