using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MySudoku;

namespace SudokuTesting
{
    /// <summary>
    /// Visualizes a permutation with three different numbers in range of 1 to 9.
    /// </summary>
    public class Permutation : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// The numbers this permutation contains.
        /// </summary>
        public int[] Numbers { get; } = new int[3];

        /// <summary>
        /// The rect transform of this game object.
        /// </summary>
        public RectTransform RectTransform => _rectTransform;
        [SerializeField] private RectTransform _rectTransform;

        /// <summary>
        /// The permutation in text format { x, y, z }.
        /// </summary>
        [SerializeField] private TMP_Text _numbersText;

        /// <summary>
        /// The background of the permutation box.
        /// </summary>
        [SerializeField] private Image _background;

        // Tags for showing if this permutation is of horizontal/vertical direction.
        [SerializeField] private Image _h;
        [SerializeField] private Image _v;

        // Panel for showing sudoku grid boxes/regions where this permutation is found.
        [SerializeField] private GameObject _data;
        [SerializeField] private TMP_Text _box;

        /// <summary>
        /// The amount this permutation is repeated horizontally/vertically in the sudoku grid.
        /// </summary>
        private int _repetitions = 0;

        private readonly List<int> _boxes = new();

        /// <summary>
        /// Sets the numbers/digits for this permutation.
        /// </summary>
        /// <param name="numbers">The three digits/numbers.</param>
        public void SetNumbers(int[] numbers)
        {
            Array.Copy(numbers, Numbers, numbers.Length);
            UpdateDigitsText();
        }

        /// <summary>
        /// Sets the permutation in string format {x, y, z} for the text component.
        /// </summary>
        private void UpdateDigitsText()
            => _numbersText.text = Numbers == null || Numbers.Length == 0 ? "" : "{" + $"{Numbers[0]}, {Numbers[1]}, {Numbers[2]}" + "}";

        /// <summary>
        /// Marks/checks the permutation.
        /// </summary>
        /// <param name="horizontal">Whether the permutation is of horizontal or vertical direction.</param>
        /// <param name="box">The region or box in the sudoku grid, where this permutation is found.</param>
        /// <returns></returns>
        public bool Check(bool horizontal, int box)
        {
            // Color the permutation based on the number of its repetitions.
            _repetitions++;
            if (_repetitions == 1) _background.color = new Color(1f, 0.5f, 0.5f);
            else if (_repetitions == 2) _background.color = new Color(0.5f, 1f, 0.5f);
            else if (_repetitions == 3) _background.color = new Color(0.5f, 0.5f, 1f);

            //// Write down the number of boxes on which this permutation is present.
            _boxes.Add(box);
            if (string.IsNullOrEmpty(_box.text))_box.text = box.ToString();
            else _box.text += ", " + box.ToString();

            _data.SetActive(true);

            // Tags the permutation direction within the grid.
            if (horizontal) _h.gameObject.SetActive(true);
            else _v.gameObject.SetActive(true);

            // Returns true if the permutation has been repeated at least three times.
            // Used for stopping the coroutine of going through sudoku solutions.
            return _repetitions == 3 && _h.gameObject.activeSelf && _v.gameObject.activeSelf;
        }

        /// <summary>
        /// Unmarks/unchecks this permutation.
        /// </summary>
        public void Uncheck()
        {
            _boxes.Clear();
            _background.color = Color.white;
            _repetitions = 0;
            _h.gameObject.SetActive(false);
            _v.gameObject.SetActive(false);
            _data.SetActive(false);
            _box.text = "";
        }

        /// <summary>
        /// Show or hide the box index data when clicked.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _data.SetActive(!_data.activeSelf);
        }
    }
}