using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using UnityEngine;

namespace MySudoku
{
    public class SudokuLogger : MonoBehaviour
    {
        private List<SudokuStep> steps;

        public string Solution { get; private set; }
        public string Puzzle   { get; private set; }




        public void RegisterStep(Operation operationType, Technique appliedTechnique,
                                (int row, int col) cellIndex, int candidate, string stepDescription = "")
        {

        }

    }

    [System.Serializable]
    public enum Operation
    {
        SKIP = 0,
        FILL = 1,
        REMOVE = 2
    }

    [System.Serializable]
    public struct SudokuStep
    {
        public Operation operationType;
        public Technique appliedTechnique;
        public (int row, int col) cellIndex;
        public int candidate;
        public string description;

        public SudokuStep(Operation operationType, Technique appliedTechnique, (int row, int col) cellIndex,
                          int candidate, string stepDescription = "")
        {
            this.operationType = operationType;
            this.appliedTechnique = appliedTechnique;
            this.cellIndex = cellIndex;
            this.candidate = candidate;
            this.description = stepDescription;
        }
    }
}