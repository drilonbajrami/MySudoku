using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Class for generating a sudoku with a solution and puzzle.
    /// </summary>
    public class SudokuGenerator : MonoBehaviour
    {
        /// <summary>
        /// Creates a sudoku.
        /// </summary>
        /// <returns>Sudoku with the solution and puzzle.</returns>
        public Sudoku GenerateSudoku()
        {
            Sudoku sudoku = new Sudoku();
            GenerateSolution(sudoku);
            GeneratePuzzle(sudoku);
            return sudoku;
        }

        /// <summary>
        /// Generates the solution for the given sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku to generate the solution for.</param>
        private void GenerateSolution(Sudoku sudoku)
        {
            // Fill the diagonal boxes first (1,5,9)
            for (int i = 0; i < 9; i += 3)
                FillBox(sudoku.Solution, row: i, col: i);

            FillRemaining(sudoku.Solution, 0, 3);
        }

        /// <summary>
        /// Generates the puzzle for the given sudoku.
        /// </summary>
        /// <param name="sudoku">The sudoku to generate the puzzle for.</param>
        private void GeneratePuzzle(Sudoku sudoku)
        {
            Array.Copy(sudoku.Solution, sudoku.Puzzle, sudoku.Solution.Length);
            int count = 64;
            while (count != 0) {
                // Pick random cell
                int cellId = UnityEngine.Random.Range(0, 81);

                // extract coordinates i and j
                int row = cellId / 9;
                int col = cellId % 9;
                if (col != 0)
                    col--;

                // If not removed yet, then remove
                if (sudoku.Puzzle[row, col] != 0) {
                    count--;
                    sudoku.Puzzle[row, col] = 0;
                }
            }
        }

        /// <summary>
        /// Fills the given box of sudoku grid with randomly placed numbers, from 1 to 9.
        /// </summary>
        /// <param name="sudoku">The sudoku solution grid.</param>
        /// <param name="row">The first row of the box.</param>
        /// <param name="col">The first column of the box.</param>
        void FillBox(int[,] sudoku, int row, int col)
        {
            int num;
            for (int r = 0; r < 3; r++) {
                for (int c = 0; c < 3; c++) {
                    do {
                        num = UnityEngine.Random.Range(1, 10);
                    }
                    while (sudoku.HasNumberInBox(row, col, num));

                    sudoku[row + r, col + c] = num;
                }
            }
        }

        /// <summary>
        /// Fills the remaining empty cells of the given sudoku grid, recursively.
        /// </summary>
        /// <param name="sudoku">The sudoku solution grid.</param>
        /// <param name="row">The row of the empty cell to fill.</param>
        /// <param name="col">The column of the empty cell to fill.</param>
        /// <returns>Whether this recursive function can go forward or backwards, continue or stop.</returns>
        bool FillRemaining(int[,] sudoku, int row, int col)
        {
            if (col > 8 && row < 8) { // Next row
                row++;
                col = 0;
            }

            if (row < 3) { // Box 2 & 3
                if (col < 3)
                    col = 3;
            }
            else if (row < 6) { // Box 4 & 6
                if (col == row / 3 * 3)
                    col += 3;
            }
            else { // Box 6 & 7
                if (col == 6) {
                    row++;
                    col = 0;
                    if (row > 8) return true;
                }
            }

            for (int num = 1; num <= 9; num++) {
                if (sudoku.CanUseNumber(row, col, num)) {
                    sudoku[row, col] = num; // assign value
                    if (FillRemaining(sudoku, row, col + 1)) // Go next, if it returns true then stop
                        return true;
                    sudoku[row, col] = 0; // Reset if not available, recursive backtracking.
                }
            }
            return false;
        }
    }
}
/* Solving rules & techniques 

1. "Last free cell"          : last remaining number in a row, column and box (100% sure).

2. "Last remaining cell"     : if adjacent boxes (2x max) have the missing number of current box on specific rows & columns
                               then find the last possible cell within current box by eliminating rows & columns (100%).

3. "Last possible number"    : Check what numbers are present in the box, column and row, place the missing one (100% only)

4. "Notes" per BOX
5. "Obvious singles"         : a note appears only once in a box. (100%)
6. "Obvious pairs"           : two notes appear alone in pairs in two cells, remove the note occurrences in other cells.
7. "Obvious triples"         : three notes appear only in three cells, remove the note occurrences in other cells.

8. "Hidden singles"          : a note appears only once in a box despite other notes.
9. "Hidden pairs"            : two notes appear only in two cells, remove other notes within these two cells. 
10. "Hidden triples"         : three notes appear only in three cells (comb of 2), remove other notes within these three cells.

11. "Pointing pairs"         : the same note appears twice in the same row or column, 
                               remove all identical notes in the same row or column in adjacent boxes.
12. "Pointing triples"       : the same note appears thrice in the same row or column, 
                               remove all identical notes in the same row or column  in adjacent boxes.

13. "X-wing"                 : on two different boxes, the same note appears on the same column and rows (diagonally), 
                               only one direction of the X can contain these notes, so remove all occurrences of these notes in 
                               other boxes.
14. "Y-wing"                 : Find a cell (pivot) with only two notes, find two other cells (pincers) in the same column and row 
                               with only two notes where at least one note is the same with one of the notes in the pivot cells.
                               Two pincer cells should have at least one identical note that is not present in the pivot cell.
                               Find the cross cell between two pincer cells and remove the common note from that cell.
15. "Swordfish"              : same X-wing but with three numbers. The same note (number) appears aligned perfectly  in 3 columns
                               or rows. 
*/