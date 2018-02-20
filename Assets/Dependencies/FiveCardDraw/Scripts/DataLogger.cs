using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DataLogger
{
    protected static bool enabled = true;            // TODO: Remember to change before release!!

    private static DataLogger instance;
    public static DataLogger Get()
    {
        if (instance == null) {
            instance = new DataLogger();
            instance.Initialize();
        }
        return instance;
    }

    protected string logsFolderName;
    private string fullFilePath;
    protected string nextOutput = "";

    protected Dictionary<string, string> uniqueLabels;
    
    protected DataLogger()
    {
        logsFolderName = "FCD_Logs";
        uniqueLabels = new Dictionary<string, string>();
    }

    protected void Initialize()
    {
        if (!enabled)
            return;
        
        string logsDirectory = Application.persistentDataPath + "/" + logsFolderName + "/ ";
        //Debug.Log(logsDirectory);

        if (!Directory.Exists(logsDirectory))
            Directory.CreateDirectory(logsDirectory);

        DirectoryInfo di = new DirectoryInfo(logsDirectory);
        FileInfo[] existingLogs = di.GetFiles("*.log", SearchOption.TopDirectoryOnly);
        string dateTimeNow = System.DateTime.Now.ToString("ddMMyyyy");

        int todayLogsCount = 1;
        for (int i = 0; i < existingLogs.Length; i++)
        {
            if (existingLogs[i].Name.Contains(dateTimeNow))
                todayLogsCount++;
        }

        fullFilePath = logsDirectory + (existingLogs.Length + 1) + "-" + dateTimeNow + "(" + todayLogsCount + ").log";
        File.Create(fullFilePath).Close();
    }

    public virtual void AddToLog(string label, string value)
    {
        if (!enabled)
            return;

        nextOutput += "  " + label + ":\t" + value + "\r\n";
    }

    public void Log(string appendLabel = "Log")
    {
        if (!enabled)
            return;

        string timeStamp = "<---- " + appendLabel + " (" + System.DateTime.Now.ToString("HH:mm:ss") + ") ---->";

        using (StreamWriter sw = File.AppendText(fullFilePath)) {
            sw.WriteLine(timeStamp + "\r\n" + nextOutput);
        }

        nextOutput = "";
    }

    public void CleanUp()
    {
        FileInfo fileInfo = new FileInfo(fullFilePath);
        if (fileInfo != null && fileInfo.Length == 0) {
            File.Delete(fullFilePath);
        }
    }
}
