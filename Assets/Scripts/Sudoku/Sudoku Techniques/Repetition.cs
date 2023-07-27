using System.Collections.Generic;

namespace MySudoku
{
    /// <summary>
    /// Class for storing data for cells or candidates(notes): <br/>
    /// 1. If used for cells: <br/>
    ///    <see cref="Index"/> is used for storing the cell's index within its respective box, row and column. <br/>
    ///    Its collections are used for storing the candidate's index which is active in this cell.
    ///    <see cref="Box"/> respective to the box it is in.<br/>
    ///    <see cref="Row"/> respective to the row it is in.<br/>
    ///    <see cref="Col"/> respective to the column it is in.<br/>
    ///    <br/>
    /// 2. If used for candidates:<br/>
    ///    <see cref="Index"/> is used for referring to the candidate it represents.<br/>
    ///    Its collections are used for storing the cell's index in which this candidate is active:<br/>
    ///    <see cref="Box"/> respective to the cell's index within its box.<br/>
    ///    <see cref="Row"/> respective to the cell's index within its row.<br/>
    ///    <see cref="Col"/> respective to the cell's index within its column.
    /// </summary>
    public class Repetition
    {
        /// <summary>
        /// The cell/candidate's index.
        /// </summary>
        public int Index { get; private set; }

        public List<List<int>> Repetitions { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">Index of the cell or candidate.</param>
        public Repetition(int index, int numberOfSets)
        {
            Index = index;
            Repetitions = new List<List<int>>(numberOfSets);
            for (int i = 0; i < numberOfSets; i++)
                Repetitions.Add(new List<int>());
        }

        /// <summary>
        /// Clears all data.
        /// </summary>
        public void ClearAll()
        {
            for (int i = 0; i < Repetitions.Count; i++)
                Repetitions[i].Clear();
        }
    }
}