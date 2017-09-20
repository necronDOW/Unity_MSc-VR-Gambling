using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerWrapper : MonoBehaviour
{
    public Logger.DataPath pathType = Logger.DataPath.Default;
    public bool consoleDebugging = true;

    private bool recordResponseTime = false;
    private float responseTimer = 0.0f;

    private void Awake()
    {
        Logger.Instance.ChangeDataPath(pathType);
        Logger.Instance.consoleDebugging = consoleDebugging;
    }

    private void Update()
    {
        if (recordResponseTime)
            responseTimer += Time.deltaTime;
    }

    public int CreateLogFile(string identifier, params string[] labels)
    {
        return Logger.Instance.NewLogFile(identifier, labels);
    }

    public void LogResponseTime(int logFileIndex, bool reset = true, bool newline = false)
    {
        Logger.Instance.LogData(responseTimer * 100.0f, logFileIndex, newline);

        if (reset)
            responseTimer = 0.0f;
    }

    public void LogClickPoint(int logFileIndex, bool newLine = false)
    {
        Logger.Instance.LogData(Input.mousePosition.x / Screen.width, logFileIndex, newLine);
        Logger.Instance.LogData(Input.mousePosition.y / Screen.height, logFileIndex, newLine);
    }

    public void LogBool(int logFileIndex, bool value, bool newline = false)
    {
        Logger.Instance.LogData(value.ToString(), logFileIndex, newline);
    }

    public void RecordResponseTime(bool value)
    {
        if (!value)
            responseTimer = 0.0f;
        recordResponseTime = value;
    }
}
