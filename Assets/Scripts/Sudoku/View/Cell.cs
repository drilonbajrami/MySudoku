using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MySudoku
{
    /// <summary>
    /// Represents a cell in the sudoku grid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        private (int row, int col) _index;

        public (int row, int col) Index {
            get => _index;
            set {
                _index = value;
                gameObject.name = $"[{_index.row},{_index.col}]";
            }
        }

        /// <summary>
        /// The number this cell contains.
        /// </summary>
        public int Number { get; private set; } = 0;

        /// <summary>
        /// Rect transform of this game object.
        /// </summary>
        [field: SerializeField] public RectTransform RectTransform { get; private set; }

        [SerializeField] private Button _button;

        public event Action<(int row, int col)> OnClicked;

        public void OnClick() => OnClicked?.Invoke(_index);
        
        /// <summary>
        /// Cell background.
        /// </summary>
        [SerializeField] private Image _background;

        /// <summary>
        /// Text representation of this cell's digit.
        /// </summary>
        [SerializeField] private TMP_Text _numberText;

        /// <summary>
        /// The notes for this cell.
        /// </summary>
        [SerializeField] private Notes _notesView;

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
            _numberText.text = num == 0 ? "" : $"{num}";
        }

        /// <summary>
        /// Shows the given number in the notes of this cell.
        /// </summary>
        /// <param name="number">The number to show.</param>
        public void ShowNote(int number, bool show)
        {
            if (show) _notesView.Show(number);
            else _notesView.Hide(number);
        }

        /// <summary>
        /// Selects this cell by marking it and its neighbor cells.
        /// </summary>
        public void Select(Color selectedColor, Action<bool> highlightNeighbors)
        {
            _background.color = selectedColor;
            highlightNeighbors?.Invoke(true);
        }

        /// <summary>
        /// Deselects this cell by umarking it and its neighbor cells.
        /// </summary>
        public void Deselect(Action<bool> highlightNeighbors)
        {
            _background.color = Color.white;
            highlightNeighbors?.Invoke(false);
        }

        /// <summary>
        /// Highlights this cell if focus is set to true.
        /// </summary>
        public void SetFocus(Color highlightColor, bool focus)
            => _background.color = focus ? highlightColor : Color.white;
    }
}