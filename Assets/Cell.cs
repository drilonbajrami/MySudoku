using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a cell in the sudoku grid.
/// </summary>
public class Cell : MonoBehaviour
{
    /// <summary>
    /// The number this cell contains.
    /// </summary>
    public int Digit => _digit;
    private int _digit = 0;

    /// <summary>
    /// Text representation of this cell's digit.
    /// </summary>
    [SerializeField] private TMP_Text _digitText;

    /// <summary>
    /// Rect transform of this game object.
    /// </summary>
    public RectTransform RectTransform => _rectTransform;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Image _background;

    public GridSums Sums;

    /// <summary>
    /// Sets this cell's digit/number.
    /// </summary>
    /// <param name="digit">The digit/number to set.</param>
    public void SetDigit(int digit)
    {
        _digit = digit;
        _digitText.text = digit == 0 ? "" : digit.ToString();
    }

    /// <summary>
    /// Toggles on/off the sums.
    /// </summary>
    public void ToggleSums() => Sums.gameObject.SetActive(!Sums.gameObject.activeSelf);

    public void Select() => _background.color = new Color(193/255f, 210/255f, 1f);
    public void Deselect() => _background.color = Color.white;

    public void Highlight(bool condition)
        => _background.color = condition ? new Color(220 / 255f, 220 / 255f, 220 / 255f) : Color.white;
}