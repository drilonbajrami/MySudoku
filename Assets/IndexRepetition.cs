using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndexRepetition : MonoBehaviour
{
    private int[] _xRepetitions = new int[9];
    private int[] _yRepetitions = new int[8];
    private int[] _zRepetitions = new int[7];

    private TextValue[] _xValues = new TextValue[9];
    private TextValue[] _yValues = new TextValue[9];
    private TextValue[] _zValues = new TextValue[9];

    private void Awake()
    {
        for(int x = 1; x < 10; x++)
        {
            _xValues[x - 1] = transform.GetChild(x).transform.GetChild(1).GetComponent<TextValue>();
            _yValues[x - 1] = transform.GetChild(x).transform.GetChild(2).GetComponent<TextValue>();
            _zValues[x - 1] = transform.GetChild(x).transform.GetChild(3).GetComponent<TextValue>();
        }
    }
    public void RegisterIndex(int[] permutationIndex)
    {
        _xRepetitions[permutationIndex[0]]++;
        _yRepetitions[permutationIndex[1]]++;
        _zRepetitions[permutationIndex[2]]++;
    }

    public void UpdateRegisterTable()
    {
        for (int x = 0; x < 9; x++)
            _xValues[x].Text = _xRepetitions[x].ToString();

        for (int y = 0; y < 8; y++)
            _yValues[y].Text = _yRepetitions[y].ToString();

        for (int z = 0; z < 7; z++)
            _zValues[z].Text = _zRepetitions[z].ToString();
    }

    public void ClearRegister()
    {
        for (int x = 0; x < 9; x++)
            _xRepetitions[x] = 0;

        for (int y = 0; y < 8; y++)
            _yRepetitions[y] = 0;

        for (int z = 0; z < 7; z++)
            _zRepetitions[z] = 0;
    }
}