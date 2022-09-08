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
    [SerializeField] private TMP_Text _crossSum;
    [SerializeField] private TMP_Text _NWtoSESum;
    [SerializeField] private TMP_Text _SWtoNESum;

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

    public void SetCrossSum(int crossSum)
    {
        _crossSum.gameObject.SetActive(true);
        _crossSum.gameObject.transform.parent.gameObject.SetActive(true);
        _crossSum.text = crossSum.ToString();
    }

    public void SetNWtoSESum(int NWtoSESum)
    {
        _NWtoSESum.gameObject.SetActive(true);
        _NWtoSESum.gameObject.transform.parent.gameObject.SetActive(true);
        _NWtoSESum.text = NWtoSESum.ToString();
    }

    public void SetSWtoNESum(int SWtoNESum)
    {
        _SWtoNESum.gameObject.SetActive(true);
        _SWtoNESum.gameObject.transform.parent.gameObject.SetActive(true);
        _SWtoNESum.text = SWtoNESum.ToString();
    }
}
