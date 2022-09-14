using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextValue : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public string Text { get { return _text.text; } 
                         set { _text.text = value; }
    }
}
