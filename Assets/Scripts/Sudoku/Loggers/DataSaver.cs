using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataSaver : MonoBehaviour
{
    private StreamWriter csvWriter;
    private string csvFilePath;

    [Header("Data Saving Settings:")]
    [Tooltip("The name of the .csv file to save the data in.")]
    public string folderPath = "";
    public string csvFileName = "";
    public bool saveData = false;

    [SerializeField] private List<string> headerColumns;

    void Awake() => SetUpDatabase();

    private void OnDestroy() => CloseDatabse();

    public void SetUpDatabase()
    {
        // Set the file path for the CSV file
        csvFilePath = Path.Combine(Application.dataPath, folderPath, $"{csvFileName}.csv");
        bool isValidPath = !string.IsNullOrEmpty(csvFileName) && Directory.Exists(Path.GetDirectoryName(csvFilePath));

        if (!isValidPath ) {
            Debug.LogWarning("Could not setup database since the given path is invalid or no file name has been given.");
            return;
        }

        CloseDatabse();

        // Open the CSV file for writing
        csvWriter = new StreamWriter(csvFilePath, true);

        // Write the header row if the file is newly created
        if (csvWriter.BaseStream.Length == 0)
            csvWriter.WriteLine(string.Join(",", headerColumns));
    }

    public void CloseDatabse()
    {
        if (csvWriter == null) return;
        csvWriter.Close();
        csvWriter.Dispose();
    }

    public void WriteNewLine(string newLine)
    {
        if(csvWriter == null || !saveData) return;
        csvWriter.WriteLine(newLine);
        csvWriter.Flush();
    }
}