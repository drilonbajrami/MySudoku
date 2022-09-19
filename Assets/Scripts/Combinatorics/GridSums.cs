using TMPro;
using UnityEngine;

/// <summary>
/// Holds the sums of columns, rows, crosses and diagonals in a sudoku box/region (per cell).
/// </summary>
public class GridSums : MonoBehaviour
{
    // Text formats of the column, row, cross and diagonal sums.
    [SerializeField] private TMP_Text _columnSum;
    [SerializeField] private TMP_Text _rowSum;
    [SerializeField] private TMP_Text _crossSum;
    [SerializeField] private TMP_Text _NWtoSESum;
    [SerializeField] private TMP_Text _SWtoNESum;

    /// <summary>
    /// Sets the column sum.
    /// </summary>
    public void SetColumnSum(int sum)
    {
        _columnSum.gameObject.SetActive(true);
        _columnSum.gameObject.transform.parent.gameObject.SetActive(true);
        _columnSum.text = sum.ToString();
    }

    /// <summary>
    /// Sets the row sum.
    /// </summary>
    public void SetRowSum(int rowSum)
    {
        _rowSum.gameObject.SetActive(true);
        _rowSum.gameObject.transform.parent.gameObject.SetActive(true);
        _rowSum.text = rowSum.ToString();
    }

    /// <summary>
    /// Sets the cross sum.
    /// </summary>
    public void SetCrossSum(int crossSum)
    {
        _crossSum.gameObject.SetActive(true);
        _crossSum.gameObject.transform.parent.gameObject.SetActive(true);
        _crossSum.text = crossSum.ToString();
    }

    /// <summary>
    /// Sets the top left to bottom right diagonal sum.
    /// </summary>
    public void SetTLBRSum(int TLBRSum)
    {
        _NWtoSESum.gameObject.SetActive(true);
        _NWtoSESum.gameObject.transform.parent.gameObject.SetActive(true);
        _NWtoSESum.text = TLBRSum.ToString();
    }

    /// <summary>
    /// Sets the bottom left to top right diagonal sum.
    /// </summary>
    public void SetBLTRSum(int BLTRSum)
    {
        _SWtoNESum.gameObject.SetActive(true);
        _SWtoNESum.gameObject.transform.parent.gameObject.SetActive(true);
        _SWtoNESum.text = BLTRSum.ToString();
    }
}
