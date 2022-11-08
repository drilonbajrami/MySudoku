using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Tilemaps;
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
                    if (applicable) SudokuGenerator.NS++;
                    break;
                case 1:
                    applicable = sudoku.HiddenSingle(notes);
                    if (applicable) SudokuGenerator.HS++;
                    break;
                case 2:
                    applicable = sudoku.CandidateLines(notes);
                    if (applicable) SudokuGenerator.CL++;
                    break;
                case 3:
                    applicable = sudoku.MultipleLines(notes);
                    if (applicable) SudokuGenerator.ML++;

                    break;
                case 4:
                    //applicable = sudoku.MultipleLines(notes);
                    applicable = false;
                    break;
                case 5:
                    applicable = sudoku.NakedPair(notes);
                    if (applicable) SudokuGenerator.NP++;
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
                        Debug.Log($"NAKED SINGLE: Cell ({row}, {col}) for {candidate}");
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
                        bool isHiddenInBox = true;
                        bool isHiddenInRow = true;
                        bool isHiddenInCol = true;

                        // First check if it is a hidden single within the box/region, row or column of the current cell.
                        for (int j = 0; j < 9; j++) {
                            int nRow = row - (row % 3) + (j / 3); // neighbor box row.
                            int nCol = col - (col % 3) + (j % 3); // neighbor box column.             

                            bool isBoxNeighbor = row != nRow || col != nCol;
                            bool isRowNeighbor = col != j;
                            bool isColNeighbor = row != j;

                            // Box, row and column
                            if (isHiddenInBox && isBoxNeighbor && sudoku[nRow, nCol] == 0) isHiddenInBox = !notes[nRow * 9 + nCol, i];
                            if (isHiddenInRow && isRowNeighbor && sudoku[row, j] == 0) isHiddenInRow = !notes[row * 9 + j, i];
                            if (isHiddenInCol && isColNeighbor && sudoku[j, col] == 0) isHiddenInCol = !notes[j * 9 + col, i];

                            isHiddenSingle = isHiddenInBox || isHiddenInRow || isHiddenInCol;
                            
                            // Check if the current note is still a hidden single when processing all neighbor cells.
                            if (isHiddenSingle && j == 8) {
                                sudoku[row, col] = i + 1;
                                notes.UpdateNotes(sudoku, (row, col), 0, i + 1);
                                Debug.Log($"HIDDEN SINGLE: Cell ({row}, {col}) for {i + 1}: {(isHiddenInBox ? "B" : "")} {(isHiddenInRow ? "R" : "")} {(isHiddenInCol ? "C" : "")}");
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
                            bool isRowNeighbor = row == nRow && col != nCol;
                            bool isColNeighbor = row != nRow && col == nCol;

                            // If not the same as the cell being processed the skip. 
                            if (!((row != nRow || col != nCol) && sudoku[nRow, nCol] == 0)) continue;

                            // Can't be applied if same note appears on a different column and row within this box/region.
                            if (!isRowNeighbor && !isColNeighbor && notes[nRow * 9 + nCol, i]) {
                                inLineRow = inLineCol = true;
                                break;
                            }

                            inLineRow = !inLineRow && isRowNeighbor ? notes[nRow * 9 + nCol, i] : inLineRow;
                            inLineCol = !inLineCol && isColNeighbor ? notes[nRow * 9 + nCol, i] : inLineCol;
                        }

                        bool techniqueIsApplied = false;

                        List<(int, int)> cand = new List<(int, int)>();

                        // If candidate has appared more than once within a row in this box, then check for neighbor cells on the same row in other boxes.
                        if (inLineRow && !inLineCol) {
                            int currentBoxCol = col - col % 3;
                            for (int j = 0; j < 9; j++)
                                // If there is any occurrence of this candidate within row neighbour cells outside of this box then remove it.
                                if ((j < currentBoxCol || j > currentBoxCol + 2) && sudoku[row, j] == 0 && notes[row * 9 + j, i]) {
                                    notes[row * 9 + j, i] = false;
                                    techniqueIsApplied = true;
                                    cand.Add((row, j));
                                }

                            if (techniqueIsApplied) {
                                StringBuilder s = new($"CANDIDATE LINES: Cell ({row}, {col}) R{row} for {i + 1} removed from cells: ");
                                for (int x = 0; x < cand.Count; x++)
                                    s.Append($"({cand[x].Item1}, {cand[x].Item2}), ");
                                Debug.Log(s.ToString());
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
                                    cand.Add((j, col));
                                }

                            if (techniqueIsApplied) {
                                StringBuilder s = new($"CANDIDATE LINES: Cell ({row}, {col}) C{col} for {i + 1} removed from cells: ");
                                for (int x = 0; x < cand.Count; x++)
                                    s.Append($"({cand[x].Item1}, {cand[x].Item2}), ");
                                Debug.Log(s.ToString());
                            }
                        }

                        if (techniqueIsApplied) return true;
                    }
                }

            return false;
        }

        public static bool MultipleLines(this int[,] sudoku, bool[,] notes)
        {
            // Process only the diagonal boxes (1, 5, 9)
            for (int box = 0; box < 9; box += 3) {
                // Check for each note.
                for (int i = 0; i < 9; i++) {
                    // Keep track of rows where this note is present in boxes.
                    // boxPerRow[x, y] -> x -> box index && y -> row index.
                    bool[,] boxPerRow = new bool[3, 3];

                    // Keep track of columns where this note is present in boxes.
                    // boxPerCol[x, y] -> x -> box index && y -> column index.
                    bool[,] boxPerCol = new bool[3, 3];

                    // d -> row or column index.
                    // k -> index of element in a row or column.
                    for (int d = 0; d < 3; d++) {
                        for (int k = 0; k < 9; k++) {
                            if (sudoku[d + box, k] == 0)
                                boxPerRow[k / 3, d] = boxPerRow[k / 3, d] || notes[(d + box) * 9 + k, i];

                            if (sudoku[k, d + box] == 0)
                                boxPerCol[k / 3, d] = boxPerCol[k / 3, d] || notes[k * 9 + d + box, i];
                        }
                    }

                    StringBuilder s = new();
                    // Check if this technique is applicable on the row direction of the currently processed boxes/regions.
                    if (IsMultipleLinesApplicable(boxPerRow, out int toBoxR, out int row)) {
                        s.Append($"MULTIPLE LINES: BOX {box + toBoxR} - Keep-ROW {row + box} for note {i + 1} -> removing from: ");
                        for (int r = 0; r < 3; r++) {
                            if (r != row)
                                for (int c = 0; c < 3; c++) {
                                    notes[(box + r) * 9 + toBoxR * 3 + c, i] = false;
                                    s.Append($"({box + r}, {toBoxR * 3 + c}), ");
                                }
                        }
                        Debug.Log(s.ToString());
                        return true;
                    }

                    // Check if this technique is applicable on the column direction of the currently processed boxes/regions.
                    if (IsMultipleLinesApplicable(boxPerCol, out int toBoxC, out int col)) {
                        s.Append($"MULTIPLE LINES: BOX {box + toBoxC} - Keep-COL {col + box} for note {i + 1} -> removing from: ");
                        for (int c = 0; c < 3; c++) {
                            if (c != col)
                                for (int r = 0; r < 3; r++) {
                                    notes[(toBoxC * 3 + r) * 9 + box + c, i] = false;
                                    s.Append($"({toBoxC * 3 + r}, {box + c}), ");
                                }
                        }
                        Debug.Log(s.ToString());
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the <see cref="MultipleLines(int[,], bool[,])"/> can be applied to the given boxes, on a row or column direction.
        /// </summary>
        /// <param name="boxs">The boxes/regions to check for.</param>
        /// <param name="toBox">The box where there should be removal of a note.</param>
        /// <param name="directionToKeep">The row/column to keep, by removing the notes on the other two remaining rows/columns.</param>
        /// <returns>Whether the technique is applicable, and the box to edit the notes int and the row or column to keep the notes in.</returns>
        private static bool IsMultipleLinesApplicable(bool[,] boxs, out int toBox, out int directionToKeep)
        {
            // 'p' and 'q' are for caching the indexes of the boxes that should not be changed,
            // in other words they are the same in terms of a note occurrence on the same rows/columns of these boxes.
            int p = -1;
            int q = -1;

            // 'toBox' is for caching the index of the box that needs to be changed/edited.
            toBox = -1;

            // It is used to cache the index of the row/column of the 'toBox',
            // based on this the note occurrences in the two remaining rows/columns will be removed.
            directionToKeep = -1;

            // Add up the number of note occurrences in rows/columns for each box. 
            int[] boxSums = new int[3];
            for(int i = 0; i < 3; i++)
                boxSums[i] = (boxs[i, 0] ? 1 : 0) + (boxs[i, 1] ? 1 : 0) + (boxs[i, 2] ? 1 : 0);

            // If at least each box contains the same note in more than one row/column,
            // and the total number of times a note appears on rows/columns in all three boxes should be at between 6 or 7 times.
            // The reason that the sum of all occurrences is checked only for 7,
            // is that the first condition of each box having a number of occurrences higher than 2 will dictate if it is equal to or greater than 6. 
            // The for loop checks the possiblites of equality between boxes in this order by their indexes ->
            // box[p] == box[q] - where (p,q) => (0,1), (1, 0), (2,0).
            if (boxSums[0] > 1 && boxSums[1] > 1 && boxSums[2] > 1 && (boxSums[0] + boxSums[1] + boxSums[2]) <= 7) {
                for(int i = 0; i < 3; i++) {
                    p = i % 3;
                    q = (i + 1) % 3;
                    if (boxs[p, 0] == boxs[q, 0] && boxs[p, 1] == boxs[q, 1] && boxs[p, 2] == boxs[q, 2])
                        toBox = (i + 2) % 3;
                }

                // If no box has been assigned as the box to edit then skip everything. It is not possible to apply this technique anymore.
                if (toBox == -1) return false;

                // Once the index of a box to change is assigned, then check for which row/column should not be edited in that particular box.
                for (int i = 0; i < 3; i++)
                    if (boxs[toBox, i] == (!boxs[p, i] || !boxs[q, i]) == true)
                        directionToKeep = i;

                return directionToKeep != -1;
            }
            else return false;
        }

        public static bool NakedPair(this int[,] sudoku, bool[,] notes)
        {
            for(int row = 0; row < 9; row++)
                for(int col = 0; col < 9; col++) {
                    if (!(sudoku[row, col] == 0)) continue;

                    StringBuilder s = new StringBuilder();

                    // Check if this cell contains a pair of candidates.
                    if(HasNakedPair(notes, (row, col), out (int, int) nakedPair)) {                
                        // Store this cell's box row and column coordinates.
                        int boxRow = row - row % 3;
                        int boxCol = col - col % 3;

                        // Keep track if the technique has been applied.
                        bool techniqueApplied = false;

                        // Store coordinates for the possible naked pair cell candidates.
                        (int r, int c) inBox = new(-1, -1);
                        int inRowColumn = -1;
                        int inColRow = -1;

                        //Debug.Log($"Cell({row}, {col}) has naked pair [{nakedPair.Item1 + 1}, {nakedPair.Item2 + 1}]... Checking neighbours...");
                        s.Append($"Cell({row}, {col}) has naked pair [{nakedPair.Item1 + 1}, {nakedPair.Item2 + 1}]... Checking neighbours...\n");

                        // First check if there is already a naked pair candidate cell assigned for the box, row and column.
                        // If no candidate cell assigned yet, then check if current cell is a naked pair candidate.
                        // If yes, then check if the naked pair is identical to the main cell.
                        // If yes than assign that cell's coordinates respective to its box, row or column.
                        for (int k = 0; k < 9; k++) {
                            if (inBox == (-1, -1) && HasNakedPair(notes, (boxRow + k / 3, boxCol + k % 3), out (int, int) nakedPairInBox))
                                if (nakedPair == nakedPairInBox && (row, col) != (boxRow + k / 3, boxCol + k % 3)) { 
                                    inBox = (boxRow + k / 3, boxCol + k % 3);
                                    s.Append($"BOX Cell({boxRow + k / 3}, {boxCol + k % 3}) with naked pair [{nakedPairInBox.Item1 + 1}, {nakedPairInBox.Item2 + 1}].\n");
                                }

                            if (inRowColumn == -1 && HasNakedPair(notes, (row, k), out (int, int) nakedPairInRow))
                                if (nakedPair == nakedPairInRow && (row, col) != (row, k)) {
                                    inRowColumn = k;
                                    s.Append($"ROW Cell({row}, {k}) with naked pair [{nakedPairInRow.Item1 + 1}, {nakedPairInRow.Item2 + 1}].\n");
                                }
                            if (inColRow == -1 && HasNakedPair(notes, (k, col), out (int, int) nakedPairInCol))
                                if (nakedPair == nakedPairInCol && (row, col) != (k, col)) {
                                    inColRow = k;
                                    s.Append($"COL Cell({k}, {col}) with naked pair [{nakedPairInCol.Item1 + 1}, {nakedPairInCol.Item2 + 1}].\n");
                                }
                        }

                        bool applicableInBox = inBox != (-1, -1);
                        bool applicableInRow = inRowColumn != -1;
                        bool applicableInCol = inColRow != -1;

                        s.Append($"Applicable in box, row, col: {applicableInBox}, {applicableInRow}, {applicableInCol} \n");

                        if (applicableInBox || applicableInRow || applicableInCol) {
                            for(int k = 0; k < 9; k++) {
                                if(applicableInBox) {
                                    int kRow = boxRow + k / 3;
                                    int kCol = boxCol + k % 3;
                                    bool isNakedPairCellOrNotEmpty = (row, col) == (kRow, kCol) || inBox == (kRow, kCol) || sudoku[kRow, kCol] != 0;

                                    if(!isNakedPairCellOrNotEmpty) {
                                        if (notes[kRow * 9 + kCol, nakedPair.Item1]) {
                                            s.Append($"BOX: Removed note {nakedPair.Item1 + 1} from cell({kRow}, {kCol}).\n");
                                            notes[kRow * 9 + kCol, nakedPair.Item1] = false;
                                            techniqueApplied = true;
                                        }

                                        if (notes[kRow * 9 + kCol, nakedPair.Item2]) {
                                            s.Append($"BOX: Removed note {nakedPair.Item2 + 1} from cell({kRow}, {kCol}).\n");
                                            notes[kRow * 9 + kCol, nakedPair.Item2] = false;
                                            techniqueApplied = true;
                                        }
                                    }
                                }

                                if (applicableInRow) {
                                    bool isNakedPairCellOrNotEmpty = col == k || inRowColumn == k || sudoku[row, k] != 0;

                                    if (!isNakedPairCellOrNotEmpty) {
                                        if (notes[row * 9 + k, nakedPair.Item1]) {
                                            s.Append($"ROW: Removed note {nakedPair.Item1 + 1} from cell({row}, {k}).\n");
                                            notes[row * 9 + k, nakedPair.Item1] = false;
                                            techniqueApplied = true;
                                        }

                                        if (notes[row * 9 + k, nakedPair.Item2]) {
                                            s.Append($"ROW: Removed note {nakedPair.Item2 + 1} from cell({row}, {k}).\n");
                                            notes[row * 9 + k, nakedPair.Item2] = false;
                                            techniqueApplied = true;
                                        }
                                    }
                                }

                                if (applicableInCol) {
                                    bool isNakedPairCellOrNotEmpty = row == k || inColRow == k || sudoku[k, col] != 0;

                                    if (!isNakedPairCellOrNotEmpty) {
                                        if (notes[k * 9 + col, nakedPair.Item1]) {
                                            s.Append($"COL: Removed note {nakedPair.Item1 + 1} from cell({k}, {col}).\n");
                                            notes[k * 9 + col, nakedPair.Item1] = false;
                                            techniqueApplied = true;
                                        }

                                        if (notes[k * 9 + col, nakedPair.Item2]) {
                                            s.Append($"COL: Removed note {nakedPair.Item2 + 1} from cell({k}, {col}).\n");
                                            notes[k * 9 + col, nakedPair.Item2] = false;
                                            techniqueApplied = true;
                                        }
                                    }
                                }
                            }

                            if (techniqueApplied) {
                                Debug.Log(s.ToString());
                                Debug.Log("------------------------------");
                                return true;
                            }
                        }                        
                    }
                }

            return false;
        }

        /// <summary>
        /// Checks if the given cell has a pair of candidates in it.
        /// </summary>
        /// <param name="notes">The notes of sudoku puzzle.</param>
        /// <param name="index">Index of the cell to check for pairs.</param>
        /// <param name="nakedPair">The naked pair in the cell if available.</param>
        /// <returns>Ture if there are only two candidates in the cell, false if less or more than two candidates.</returns>
        private static bool HasNakedPair(bool[,] notes, (int row,int col) index, out (int, int) nakedPair)
        {
            nakedPair = (-1, -1);

            for (int i = 0; i < 9; i++) {
                if (notes[index.row * 9 + index.col, i]) {
                    if (nakedPair.Item1 == -1) nakedPair.Item1 = i;
                    else if (nakedPair.Item2 == -1) nakedPair.Item2 = i;
                    else return false;
                }
            }
            
            return nakedPair.Item1 != -1 && nakedPair.Item2 != -1;
        }

        public static bool HiddenPair(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool NakedTriple(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool HiddenTriple(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool XWing(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool YWing(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool ForcingChains(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool NakedQuad(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool HiddenQuad(this int[,] sudoku, bool[,] notes)
        {
            return false;
        }

        public static bool Swordfish(this int[,] sudoku, bool[,] notes)
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
            3. Candidate Lines(Pointing Pairs)  -   350 -> 200

            5. MULTIPLE LINES   -   600 -> 300;
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

            4. Double Pairs     -   500 -> 250
            5. Multiple Lines   -   700 -> 400
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