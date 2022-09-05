using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SudokuSolution : MonoBehaviour
{
    public TMP_InputField _inputField;

    public SudokuGridView _sudoku;

    public void GetSolution(string solution)
    {  
        List<int> digits = new List<int>();
        for(int i = 0; i < solution.Length; i++)
            digits.Add(int.Parse(solution[i].ToString()));
        
        if(digits.Count > 0)
            _sudoku.FillGrid(digits);
    }
}
