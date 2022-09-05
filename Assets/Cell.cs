using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    private int _digit = 0;

    public int Digit => _digit;

    [SerializeField] private RectTransform _rectTransform;

    public RectTransform RectTransform => _rectTransform;

    [SerializeField] private TMP_Text _digitText;

    [SerializeField] private TMP_Text _columnSum;
    [SerializeField] private TMP_Text _rowSum;

    public void SetDigit(int digit)
    {
        _digit = digit;
        _digitText.text = digit == 0 ? "" : digit.ToString();
    }

    public void SetColumnSum(int sum)
    {
        _columnSum.gameObject.SetActive(true);
        _columnSum.gameObject.transform.parent.gameObject.SetActive(true);
        _columnSum.text = sum.ToString();
    }
    public void SetRowSum(int rowSum)
    {
        _rowSum.gameObject.SetActive(true);
        _rowSum.gameObject.transform.parent.gameObject.SetActive(true);
        _rowSum.text = rowSum.ToString();
    }
}
