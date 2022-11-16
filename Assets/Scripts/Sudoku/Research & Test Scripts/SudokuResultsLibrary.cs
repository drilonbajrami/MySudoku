using System;
using System.Collections.Generic;
using System.IO;
using MySudoku;
using UnityEngine;

/// <summary>
/// Cycles through the sudoku solutions library files or through the files.
/// </summary>
public class SudokuResultsLibrary : MonoBehaviour
{
    // 100 000 solutions per file - 1 solution per 1 line.
    const int FIRST_LINE = 0;
    const int LAST_LINE = 99999;

    // Amount of Solution files.
    const int FIRST_FILE = 1;
    const int LAST_FILE = 90;

    // Current line in the solution file.
    private int _currentLine = 0;

    // Current file in the folder of solution files. 
    private int _currentFile = 1;

    // Folder and File paths
    private const string _folderPath = @"C:\Users\drilo\Desktop\SudokuResults\";
    private const string _fileName = "sudoku-";
    private const string _fileExtension = ".csv";

    private string[] _solutions;

    [SerializeField] private SudokuView _sudokuView;
    [SerializeField] private bool _printSolutions = true;

    // Load the first file on start.
    private void Awake()
    {
        _solutions = File.ReadAllLines(_folderPath + _fileName + _currentFile.ToString() + _fileExtension);
        if (_printSolutions) Debug.Log($"Loaded file... 'sudoku-{_currentFile}.csv'");
    }

    private void Update() => CycleSudokuSolutions();

    /// <summary>
    /// Cycles through the sudoku solution files and singe solution through key presses.
    /// </summary>
    private void CycleSudokuSolutions()
    {
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    LoadNextFile();
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //    LoadPreviousFile();
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //    _sudokuView.SetGridValues(GetNextSolution());
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    _sudokuView.SetGridValues(GetPreviousSolution());
    }

    /// <summary>
    /// Loads the next sudoku solution file.
    /// </summary>
    public void LoadNextFile()
    {
        _currentFile++;
        if (_currentFile > LAST_FILE) _currentFile = FIRST_FILE;
        _solutions = File.ReadAllLines(_folderPath + _fileName + _currentFile.ToString() + _fileExtension);
        if (_printSolutions) Debug.Log($"Loaded file... 'sudoku-{_currentFile}.csv'");
        _currentLine = FIRST_LINE;
    }

    /// <summary>
    /// Loads the perivous sudoku solution file.
    /// </summary>
    public void LoadPreviousFile()
    {
        _currentFile--;
        if (_currentFile < FIRST_FILE) _currentFile = LAST_FILE;
        _solutions = File.ReadAllLines(_folderPath + _fileName + _currentFile.ToString() + _fileExtension);
        if (_printSolutions) Debug.Log($"Loaded file... 'sudoku-{_currentFile}.csv'");
        _currentLine = FIRST_LINE;
    }

    /// <summary>
    /// Returns the current stored solutions from the sudoku solutions library.
    /// </summary>
    public int[,] GetCurrentSolution() => GetSolution(_solutions[_currentLine]);

    /// <summary>
    /// Returns the next solution within the current file of sudoku solutions library.
    /// </summary>
    public int[,] GetNextSolution()
    {
        _currentLine++;
        if (_currentLine > LAST_LINE) _currentLine = FIRST_LINE;
        if (_printSolutions) Debug.Log($"Solution {_currentLine}");
        return GetSolution(_solutions[_currentLine]);
    }

    /// <summary>
    /// Returns the previous solution within the current file of sudoku solutions library.
    /// </summary>
    public int[,] GetPreviousSolution()
    {
        _currentLine--;
        if (_currentLine < FIRST_LINE) _currentLine = LAST_LINE;
        if (_printSolutions) Debug.Log($"Solution {_currentLine}");
        return GetSolution(_solutions[_currentLine]);
    }

    /// <summary>
    /// Converts the sudoku solution from string form to a list of integers.
    /// </summary>
    /// <param name="solution">The sudoku solution in a string format.</param>
    /// <returns>The sudoku solution in a list of integers format.</returns>
    private int[,] GetSolution(string solution)
    {
        int[,] numbers = new int[9, 9];
        for (int i = 0; i < solution.Length; i++)
            numbers[i / 9, i % 9] = int.Parse(solution[i].ToString());

        return numbers;
    }
}