using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

public static class CSVReader
{
    public static List<string[]> ReadCSVFile(string filePath)
    {
        List<string[]> csvLines = new List<string[]>();

        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[] values = parser.ReadFields();
                csvLines.Add(values);
            }
        }

        return csvLines;
    }
}