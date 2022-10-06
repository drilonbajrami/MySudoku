using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Represents a cell in the sudoku grid.
/// </summary>
public class Cell : MonoBehaviour
{
    /// <summary>
    /// The number this cell contains.
    /// </summary>
    public int Number { get; private set; } = 0;

    /// <summary>
    /// Rect transform of this game object.
    /// </summary>
    public RectTransform RectTransform { get; private set; }

    /// <summary>
    /// Cell background.
    /// </summary>
    private Image _background;

    /// <summary>
    /// Text representation of this cell's digit.
    /// </summary>
    private TMP_Text _numberText;

    public GridSums Sums;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        _background = GetComponent<Image>();
        _numberText = GetComponentInChildren<TMP_Text>();
    }

    /// <summary>
    /// Sets this cell's digit/number.
    /// </summary>
    /// <param name="num">The digit/number to set.</param>
    public void SetNum(int num)
    {
        Number = num;
        _numberText.text = num == 0 ? "" : num.ToString();
    }

    /// <summary>
    /// Toggles on/off the sums.
    /// </summary>
    public void ToggleSums() => Sums.gameObject.SetActive(!Sums.gameObject.activeSelf);

    public void Select(Action<bool> highlightNeighbors)
    {
        _background.color = new Color(193 / 255f, 210 / 255f, 1f);
        highlightNeighbors?.Invoke(true);
    }
    public void Deselect(Action<bool> highlightNeighbors)
    {
        _background.color = Color.white;
        highlightNeighbors?.Invoke(false);
    }

    public void Focus(bool condition)
        => _background.color = condition ? new Color(220 / 255f, 220 / 255f, 220 / 255f) : Color.white;

    public void Focus(bool condition, Color color) => _background.color = condition ? color : Color.white;
}