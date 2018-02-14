using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
    public static bool ReadFromFile(string dir, out int[] arr)
    {
        string[] strData;
        if (!ReadFromFile(dir, out strData))
        {
            arr = new int[1] { -1 };
            return false;
        }

        arr = new int[strData.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            if (!int.TryParse(strData[i], out arr[i]))
                arr[i] = int.MinValue;
        }

        return true;
    }

    public static bool ReadFromFile(string dir, out string[] arr)
    {
        if (!File.Exists(dir))
        {
            arr = new string[1] { "NULL" };
            return false;
        }

        string rawText = File.ReadAllText(dir);
        arr = rawText.Split(',', '\n');

        return true;
    }
}
