using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Permutation : MonoBehaviour
{
    private int[] _digits = new int[3];
    public int[] Digits => _digits;

    [SerializeField] private TMP_Text _digitsText;
    [SerializeField] private Image _background;

    [SerializeField] private RectTransform _rectTransform;

    public RectTransform RectTransform => _rectTransform;

    private int _repetitions = 0;

    public void SetDigits(int[] digits)
    {
        Array.Copy(digits, _digits, digits.Length);
        UpdateDigitsText();
    }

    private void UpdateDigitsText()
        => _digitsText.text = _digits == null || _digits.Length == 0 ? "" : "{" + $"{_digits[0]}, {_digits[1]}, {_digits[2]}" + "}";

    public bool IsCompatibleWith(Permutation triplet)
    {
        for (int i = 0; i < 3; i++)
            if (Digits[i] == triplet.Digits[i])
            {
                triplet.Lock();
                return false;
            }

        return true;
    }

    public void Lock() => _background.color = Color.red;

    public void Unlock() => _background.color = Color.white;

    public void Check()
    {
        _repetitions++;
        if (_repetitions == 1) _background.color = Color.red;
        else if(_repetitions == 2) _background.color = Color.green;
        else if(_repetitions == 3) _background.color = Color.blue;
        else if(_repetitions == 4)
        {
            _background.color = Color.yellow;
            Debug.Break();
        }
        //_background.color = new Color(1f, 0.5f, 0.5f);
    }

    public void Uncheck()
    {
        _background.color = Color.white;
        _repetitions = 0;
    }
}