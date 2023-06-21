using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MySudoku
{
    [CustomEditor(typeof(SudokuView))]
    public class SudokuViewEditor : Editor
    {
        SudokuView view;

        private void OnEnable()
            => view = (SudokuView)target;
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Copy Puzzle to Clipboard"))
                view.CopyPuzzleToClipboard();

            if (GUILayout.Button("Enter Puzzle"))
                view.EnterPuzzle();
        }
    }
}