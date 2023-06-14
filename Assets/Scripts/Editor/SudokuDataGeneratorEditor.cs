using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MySudoku;

[CustomEditor(typeof(SudokuDataGenerator))]
public class SudokuDataGeneratorEditor : Editor
{
    SudokuDataGenerator dataGenerator;

    private void OnEnable()
        => dataGenerator = (SudokuDataGenerator)target;

    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI(); 
        
        if(GUILayout.Button("Start Generating"))
            dataGenerator.StartGenerations();

        if(dataGenerator.IsGenerating && GUILayout.Button("Stop Generating"))
            dataGenerator.StopGenerations();

        if (GUILayout.Button("Clear Current Data"))
            dataGenerator.DataSaver.ClearExistingData();
    }
}