using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger
{
    public bool consoleDebugging = false;

    private static Logger _instance;
    private List<string> filePaths;
    private string sessionID;
    private string dataPath;

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Logger();
            return _instance;
        }
        private set { }
    }

    private Logger()
    {
        filePaths = new List<string>();
        consoleDebugging = Application.isEditor;
        sessionID = DateTime.Now.ToString("yyyyMMddHHmmss");
        dataPath = "/logdata/";
    }

    public int NewLogFile(string identifier, params string[] labels)
    {
        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        filePaths.Add(dataPath + sessionID + "-" + identifier + ".csv");

        if (labels.Length > 0)
        {
            string labelsConcatinated = "";
            for (int i = 0; i < labels.Length - 1; i++)
                labelsConcatinated += labels[i] + ",";
            labelsConcatinated += labels[labels.Length - 1];

            LogData(labelsConcatinated, filePaths.Count - 1, true);
        }

        return filePaths.Count - 1;
    }

    public void LogData(int data, int logIndex, bool newLine = false)
    {
        LogData(data.ToString(), logIndex, newLine);
    }

    public void LogData(float data, int logIndex, bool newLine = false)
    {
        LogData(data.ToString("0.00"), logIndex, newLine);
    }

    public void LogData(string data, int logIndex, bool newLine = false)
    {
        if (logIndex >= 0 && logIndex < filePaths.Count)
        {
            TextWriter tw = new StreamWriter(filePaths[logIndex], true);
            tw.Write(data + (newLine ? tw.NewLine : ","));
            tw.Close();

            if (consoleDebugging)
                Debug.Log(data);
        }
    }

    public enum DataPath { Default, Persistent, Root }
    public void ChangeDataPath(DataPath type)
    {
        switch (type)
        {
            case DataPath.Default:
                dataPath = Application.dataPath;
                break;
            case DataPath.Persistent:
                dataPath = Application.persistentDataPath;
                break;
            case DataPath.Root:
                if (Application.platform == RuntimePlatform.Android)
                    dataPath = Application.persistentDataPath + "/../../../../gamblingapp-data";
                else dataPath = "";
                break;
        }

        dataPath += "/logdata/";
    }
}
