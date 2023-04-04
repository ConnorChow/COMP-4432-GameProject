using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Application.dataPath + "/CSV/mydata.csv";

        // Prepare the header for the CSV file
        string[] header = { "Player Name", "Health", "Max Health", "Player Speed", "Is Dead", "is_cheating\n" };

        // Read existing data from the CSV file
        List<string[]> data = ReadCSV();

        // If the file is empty, add the header
        if (data.Count == 0)
        {
            data.Add(header);
        }
    }

    List<string[]> ReadCSV()
    {
        List<string[]> data = new List<string[]>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] row = line.Split(',');
                    data.Add(row);
                }
            }
        }

        return data;
    }

    void WriteCSV(List<string[]> data)
    {
        // Create the directory if it doesn't exist
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Open a new StreamWriter
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the data
            foreach (string[] row in data)
            {
                writer.WriteLine(string.Join(",", row));
            }
        }
    }
}
