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
    private TMP_Text _text;

    /// <summary>
    /// The notes for this cell.
    /// </summary>
    [SerializeField] private Notes _notesView;

    public GridSums Sums;

    /// <summary>
    /// Caches the needed components of this cell game object.
    /// </summary>
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
        _notesView = GetComponentInChildren<Notes>();
    }
    
    /// <summary>
    /// Initializes any necessary components for this cell.
    /// </summary>
    public void Initialize()
    {
        _notesView.Initialize(RectTransform.sizeDelta.x);
        _notesView.gameObject.SetActive(Number == 0);
    }

    /// <summary>
    /// Sets this cell's number.
    /// </summary>
    /// <param name="num">The digit/number to set.</param>
    public void SetNum(int num)
    {
        Number = num;
        _notesView.gameObject.SetActive(num == 0);
        _text.text = num == 0 ? "" : num.ToString();
    }

    /// <summary>
    /// Shows the given number in the notes of this cell.
    /// </summary>
    /// <param name="number">The number to show.</param>
    public void ShowNote(int number, bool show) {
        if (show) _notesView.Show(number);
        else _notesView.Hide(number);
    }

    ///// <summary>
    ///// Hides the given number in the notes of this cell.
    ///// </summary>
    ///// <param name="number">The number to hide.</param>
    //public void HideNote(int number) => _notes.Hide(number);
    
    /// <summary>
    /// Selects this cell by marking it and its neighbor cells.
    /// </summary>
    /// <param name="highlightNeighbors"></param>
    public void Select(Color selectedColor, Action<bool> highlightNeighbors)
    {
        _background.color = selectedColor;
        highlightNeighbors?.Invoke(true);
    }

    /// <summary>
    /// Deselects this cell by umarking it and its neighbor cells.
    /// </summary>
    /// <param name="highlightNeighbors"></param>
    public void Deselect(Action<bool> highlightNeighbors)
    {
        _background.color = Color.white;
        highlightNeighbors?.Invoke(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="focusedColor"></param>
    /// <param name="condition"></param>
    public void Focus(Color focusedColor, bool condition)
        => _background.color = condition ? focusedColor : Color.white;

    /// <summary>
    /// Toggles on/off the sums.
    /// </summary>
    public void ToggleSums() => Sums.gameObject.SetActive(!Sums.gameObject.activeSelf);
}