using UnityEditor;
using UnityEngine;

namespace MySudoku
{
    [CustomEditor(typeof(SudokuGenerator))]
    public class SudokuGeneratorEditor : Editor
    {
        private SerializedProperty _solutionGenerator;
        private SerializedProperty _puzzleGenerator;
        private SerializedProperty _randomGenerator;
        private SerializedProperty _useOwnRandomGeneartor;
        private SerializedProperty _seed;
        private SerializedProperty _solutionGeneratorSeed;
        private SerializedProperty _puzzleGeneratorSeed;

        private void OnEnable()
        {
            _solutionGenerator = serializedObject.FindProperty("_solutionGenerator");
            _puzzleGenerator = serializedObject.FindProperty("_puzzleGenerator");
            _randomGenerator = serializedObject.FindProperty("_randomGenerator");
            _useOwnRandomGeneartor = serializedObject.FindProperty("_useOwnRandomGeneartor");
            _seed = serializedObject.FindProperty("_seed");
            _solutionGeneratorSeed = serializedObject.FindProperty("_solutionGeneratorSeed");
            _puzzleGeneratorSeed = serializedObject.FindProperty("_puzzleGeneratorSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_solutionGenerator);
            EditorGUILayout.PropertyField(_puzzleGenerator);
            EditorGUILayout.PropertyField(_randomGenerator);

            EditorGUILayout.Space(20);

            EditorGUILayout.PropertyField(_useOwnRandomGeneartor);

            if (_useOwnRandomGeneartor.boolValue) {
                EditorGUILayout.PropertyField(_seed, new GUIContent("Seed"));
            }
            else {
                EditorGUILayout.PropertyField(_solutionGeneratorSeed, new GUIContent("Solution Generator Seed"));
                EditorGUILayout.PropertyField(_puzzleGeneratorSeed, new GUIContent("Puzzle Generator Seed"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}