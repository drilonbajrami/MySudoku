using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Permutation : MonoBehaviour, IPointerClickHandler
{
    private int[] _digits = new int[3];
    public int[] Digits => _digits;

    [SerializeField] private TMP_Text _digitsText;
    [SerializeField] private Image _background;

    [SerializeField] private Image _h;
    [SerializeField] private Image _v;

    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private GameObject _data;
    [SerializeField] private TMP_Text _box;

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

    public bool Check(bool horizontal, int box)
    {
        _repetitions++;
        if (_repetitions == 1) _background.color = new Color(1f, 0.5f, 0.5f);
        else if (_repetitions == 2) _background.color = new Color(0.5f, 1f, 0.5f);
        else if (_repetitions == 3) _background.color = new Color(0.5f, 0.5f, 1f);

        if (string.IsNullOrEmpty(_box.text))
            _box.text = /*"Box " + */box.ToString();
        else
            _box.text += ", " + box.ToString();
        _data.gameObject.SetActive(true);


        if (horizontal) _h.gameObject.SetActive(true);
        else _v.gameObject.SetActive(true);

        if (_repetitions == 3 && _h.gameObject.activeSelf && _v.gameObject.activeSelf)
            return true;
        else
            return false;

    }

    public void Uncheck()
    {
        _background.color = Color.white;
        _repetitions = 0;
        _h.gameObject.SetActive(false);
        _v.gameObject.SetActive(false);
        _data.gameObject.SetActive(false);
        _box.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _data.gameObject.SetActive(!_data.gameObject.activeSelf);
    }
}