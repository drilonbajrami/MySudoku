using System.Collections;
using System.Collections.Generic;
using MySudoku;
using UnityEngine;

public static class NotesExtensions
{
    /// <summary>
    /// Updates the notes based on the changed cell value.
    /// </summary>
    /// <param name="notes">The notes to update.</param>
    /// <param name="sudoku">The sudoku to update the notes on.</param>
    /// <param name="cellIndex">The index of the edited cell.</param>
    /// <param name="removedValue">The removed/old value of the cell.</param>
    /// <param name="addedValue">The added/new value of the cell.</param>
    public static void UpdateNotes(this bool[,] notes, int[,] sudoku, (int row, int col) cellIndex, int removedValue, int addedValue)
    {
        bool isZero = addedValue == 0;
        int valueIndex = isZero ? removedValue - 1 : addedValue - 1;

        for (int i = 0; i < 9; i++) {
            // Disable all notes in this cell if the new value is not zero.
            if (!isZero) notes[cellIndex.row * 9 + cellIndex.col, i] = false;

            // Row
            notes[i * 9 + cellIndex.col, valueIndex] = isZero && sudoku.CanUseNumber(i, cellIndex.col, valueIndex + 1);

            // Column
            notes[cellIndex.row * 9 + i, valueIndex] = isZero && sudoku.CanUseNumber(cellIndex.row, i, valueIndex + 1);

            // Box 3x3
            int row = cellIndex.row - cellIndex.row % 3 + i / 3;
            int col = cellIndex.col - cellIndex.col % 3 + i % 3;
            notes[row * 9 + col, valueIndex] = isZero && sudoku.CanUseNumber(row, col, valueIndex + 1);
        }
    }

    /// <summary>
    /// Removes the given note from the given cell by index.
    /// </summary>
    /// <param name="notes">The notes.</param>
    /// <param name="cellIndex">Index of the cell to remove note in.</param>
    /// <param name="noteValue">The note to hide.</param>
    public static void RemoveNote(this bool[,] notes, (int row, int col) cellIndex, int noteValue) => notes[cellIndex.row * 9 + cellIndex.col, noteValue - 1] = false;

    public static void SetNotes(this bool[,] notes, int[,] puzzle)
    {
        for(int row = 0; row < 9; row++)
            for(int col = 0; col < 9; col++) {
                bool isEmpty = puzzle[row, col] == 0;

                for(int i = 0; i < 9; i++) {
                    notes[row * 9 + col, i] = isEmpty && puzzle.CanUseNumber(row, col, i + 1);
                }
            }
    }
}