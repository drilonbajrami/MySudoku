using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MySudoku
{
    /// <summary>
    /// Represents a cell in the sudoku grid.
    /// </summary>
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

        public event Action<(int row, int col)> OnClicked;
        public event Action<(int row, int col), bool> OnHovered;

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

        private Color _lastUsedColor;

        [Space(10)]
        [Header("Cell UI Colors:")]
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _neighbourSelectedColor;
        [SerializeField] private Color _sameNumberSelectedColor;
        [SerializeField] private Color _hoveredColor;
        [SerializeField] private Color _neighbourHoveredColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _background.color = _hoveredColor;
            OnHovered?.Invoke(Index, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ReverseColorSelection();
            OnHovered?.Invoke(Index, false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UpdateColorAsSelected();
            OnClicked?.Invoke(Index);
        }

        /// <summary>
        /// Initializes any necessary components for this cell.
        /// </summary>
        public void Initialize()
        {
            _lastUsedColor = _normalColor;
            _background.color = _normalColor;
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


        #region Selection UI Color Handling

        public void UpdateColorAsSelected()
        {
            _lastUsedColor = _selectedColor;
            _background.color = _selectedColor;
        }

        public void UpdateColorAsNeighbourSelected()
        {
            _lastUsedColor = _neighbourSelectedColor;
            _background.color = _neighbourSelectedColor;
        }
        
        public void UpdateColorAsSameNumber()
        {
            _lastUsedColor = _sameNumberSelectedColor;
            _background.color = _sameNumberSelectedColor;
        }

        public void UpdateColorAsNeighbourHovered()
        {
            _background.color = _neighbourHoveredColor;
        }
        
        public void ResetColorSelection()
        {
            _lastUsedColor = _normalColor;
            _background.color = _normalColor;
        }

        public void ReverseColorSelection()
            => _background.color = _lastUsedColor;
        
        #endregion
    }
}