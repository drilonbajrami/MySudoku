using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Triplet : MonoBehaviour
{
    private int[] _digits;
    public int[] Digits => _digits;

    [SerializeField] private TMP_Text _digitsText;
    [SerializeField] private Image _background;

    public void SetDigits(int[] digits)
    {
        Array.Copy(digits, _digits, digits.Length);
        UpdateDigitsText();
    }

    private void UpdateDigitsText()
        => _digitsText.text = _digits == null || _digits.Length == 0 ? "" : _digits.ToString();

    public bool IsCompatibleWith(Triplet triplet)
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
}