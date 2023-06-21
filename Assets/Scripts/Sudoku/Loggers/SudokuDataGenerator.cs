using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using MySudoku;
using UnityEngine;

namespace MySudoku
{
    [RequireComponent(typeof(DataSaver))]
    public class SudokuDataGenerator : MonoBehaviour
    {
        public SudokuGenerator generator;
        public Difficulty difficulty;
        public int numOfGenerations = 100;
        private int numOfGenerationsLeft = 0;
        public bool IsGenerating { get; private set; }
        
        private Coroutine generationCoroutine;
        public DataSaver DataSaver { get; private set; }

        void Awake()
        {
            DataSaver = GetComponent<DataSaver>();
        }

        public void WriteData(string data)
        {
            DataSaver.WriteNewLine(data);
            if (!DataSaver.saveData) return;
       
            numOfGenerationsLeft--;
            if (IsGenerating) { Debug.Log($"Puzzles left to generate: {numOfGenerationsLeft}"); }
        }

        public void StartGenerations()
        {
            IsGenerating = true;
            DataSaver.SetUpDatabase();
            numOfGenerationsLeft = numOfGenerations;
            generationCoroutine = StartCoroutine(GeneratePuzzlesCoroutine());
        }

        private IEnumerator GeneratePuzzlesCoroutine()
        {
            while (numOfGenerationsLeft > 0 && IsGenerating) {
                generator.Generate(difficulty);
                yield return null; // Yield to the next frame
            }

            IsGenerating = false;
        }

        // Add a method to stop the generation coroutine if needed
        public void StopGenerations()
        {
            IsGenerating = false;
            if (generationCoroutine != null) {
                StopCoroutine(generationCoroutine);
                DataSaver.CloseDatabse(); // Close the database if the generation was interrupted
            }
        }
    }
}