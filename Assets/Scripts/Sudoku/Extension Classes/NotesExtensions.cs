using System.Collections;
using System.Collections.Generic;
using MySudoku;
using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Extension methods for sudoku notes (bool[,]).
    /// </summary>
    public static class NotesExtensions
    {
        /// <summary>
        /// Updates the notes based on the changed cell value.
        /// </summary>
        /// <param name="notes">The notes to update.</param>
        /// <param name="sudoku">The sudoku to update the notes on.</param>
        /// <param name="cellIndex">The index of the edited cell.</param>
        /// <param name="removedValue">The removed/old value of the cell.</param>
        /// <param name="placedValue">The added/new value of the cell.</param>
        public static void Update(this bool[,] notes, int[,] sudoku, (int row, int col) cellIndex, int removedValue, int placedValue)
        {
            bool isZero = placedValue == 0;
            int noteIndex = (isZero ? removedValue : placedValue) - 1;

            (int row, int col) boxCoords = new(cellIndex.row - cellIndex.row % 3, cellIndex.col - cellIndex.col % 3);

            int cellIndexInNotes = cellIndex.row * 9 + cellIndex.col;

            //List<(int row, int col)> indexes = new List<(int row, int col)>();

            for (int i = 0; i < 9; i++) {
                // Disable all notes in this cell if the new value is not zero.
                if (!isZero) notes[cellIndex.row * 9 + cellIndex.col, i] = false;

                // Box 3x3
                int row = boxCoords.row + i / 3;
                int col = boxCoords.col + i % 3;

                // All cells within the box, and the edited cell itself.
                notes[row * 9 + col, noteIndex] = isZero && sudoku.CanUseNumber(row, col, noteIndex + 1);

                // Row cells, only the ones outside of the box.
                if (i < boxCoords.row || i > boxCoords.row + 2) {
                    notes[i * 9 + cellIndex.col, noteIndex] = isZero && sudoku.CanUseNumber(i, cellIndex.col, noteIndex + 1);
                }

                // Column cells, only the ones outside of the box.
                if (i < boxCoords.col || i > boxCoords.col + 2) {
                    notes[cellIndex.row * 9 + i, noteIndex] = isZero && sudoku.CanUseNumber(cellIndex.row, i, noteIndex + 1);
                }
            }
        }

        /// <summary>
        /// Sets sudoku notes based on the given sudoku puzzle.
        /// </summary>
        /// <param name="notes">The notes to set.</param>
        /// <param name="puzzle">The sudoku puzzle.</param>
        public static void SetNotes(this bool[,] notes, int[,] puzzle)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++) {
                    bool isEmpty = puzzle[row, col] == 0;

                    for (int i = 0; i < 9; i++) {
                        notes[row * 9 + col, i] = isEmpty && puzzle.CanUseNumber(row, col, i + 1);
                    }
                }
        }

        /// <summary>
        /// Updates the sudoku notes based on the changed cell, its box, row and column.
        /// Does not update notes if an already filled cell is cleared.
        /// </summary>
        /// <param name="notes">The notes to update.</param>
        /// <param name="cellIndex">The index of the edited cell.</param>
        /// <param name="newValue">The added/new value of the cell.</param>
        public static void UpdateOnCellEdit(this bool[,] notes, (int row, int col) cellIndex, int newValue)
        {
            if (newValue == 0) return;
            int noteIndex = newValue - 1;
            for (int i = 0; i < 9; i++) {
                // Disable all notes in this cell if the new value is not zero.
                notes[cellIndex.row * 9 + cellIndex.col, i] = false;

                // Disable this number note on all other neighbor cells.
                int row = cellIndex.row - cellIndex.row % 3 + i / 3;
                int col = cellIndex.col - cellIndex.col % 3 + i % 3;
                notes[i * 9 + cellIndex.col, noteIndex] = notes[cellIndex.row * 9 + i, noteIndex] = notes[row * 9 + col, noteIndex] = false;
            }
        }
    }
}