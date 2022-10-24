using UnityEngine;

/// <summary>
/// Takes care of showing/hidding the numbers in the notes of a cell.
/// </summary>
public class Notes : MonoBehaviour
{
    /// <summary>
    /// The rect transform of the numbers game objects.
    /// </summary>
    [SerializeField] private RectTransform[] _numbers;

    /// <summary>
    /// Resizes and rearranges the positioning of the note's numbers within the cell,
    /// based on the cell's size.
    /// </summary>
    public void Initialize(float cellSize)
    {
        float noteSize = cellSize / 3f;
        for(int i = 0; i < 9; i++) {
            int row = i / 3;
            int col = i % 3;
            _numbers[i].sizeDelta = new Vector2(noteSize, noteSize);
            _numbers[i].anchoredPosition = new Vector2(col * noteSize, -row * noteSize);
            _numbers[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the given number in the notes.
    /// </summary>
    /// <param name="number">The number to show.</param>
    public void Show(int number)
    {
        if(0 < number && number < 10)
            _numbers[number - 1].gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the given numbers in the notes.
    /// </summary>
    /// <param name="numbers">The numbers to show.</param>
    public void Show(int[] numbers)
    {
        foreach(int number in numbers)
            Show(number);
    }

    /// <summary>
    /// Hides the given number in the notes.
    /// </summary>
    /// <param name="number">The number to hide.</param>
    public void Hide(int number)
    {
        if (0 < number && number < 10)
            _numbers[number - 1].gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides the given numbers in the notes.
    /// </summary>
    /// <param name="numbers">The numbers to hide.</param>
    public void Hide(int[] numbers)
    {
        foreach (int number in numbers)
            Hide(number);
    }
}