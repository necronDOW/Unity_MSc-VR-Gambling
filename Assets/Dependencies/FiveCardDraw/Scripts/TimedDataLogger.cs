using UnityEngine;

public class TimedDataLogger : DataLogger
{
    private static TimedDataLogger instance;
    public static new TimedDataLogger Get()
    {
        if (instance == null) {
            instance = new TimedDataLogger();
            instance.Initialize();
        }
        return instance;
    }

    private float lastLogSeconds = 0.0f;
    private bool optimizeForParsing = false;
    private bool firstLogOccured = false;

    protected TimedDataLogger()
    {
        logsFolderName = "FCD_Logs/Event";
        optimizeForParsing = true;
    }
    
    /// <summary>
    /// Adds a labelled time to the log. The time used is based on the time of the last log.
    /// </summary>
    /// <param name="label">The message to affix to the time.</param>
    /// <param name="logID">Used when optimizeForParsing is enabled. Adds a prefixed log id as an integer to the start of the line. If left blank, label will be used to search unique ids.</param>
    public override void AddToLog(string label, string logID = "")
    {
        if (!enabled)
            return;

        if (!firstLogOccured) {
            OverrideLastRecordedTime();
            firstLogOccured = true;
        }

        float timeSinceLastLog = Time.realtimeSinceStartup - lastLogSeconds;
        string appendMathSymbol = (timeSinceLastLog > 0.0f) ? "+" : "";

        if (logID == "") {
            logID = label;
        }

        nextOutput += "  " + UniqueLogID(logID) + "(" + appendMathSymbol + string.Format("{0:f10}s", timeSinceLastLog) + ") " + label + "\r\n";
        OverrideLastRecordedTime();
    }

    public void OverrideLastRecordedTime()
    {
        lastLogSeconds = Time.realtimeSinceStartup;
    }

    private string UniqueLogID(string logID)
    {
        if (optimizeForParsing) {
            if (!uniqueLabels.ContainsKey(logID)) {
                uniqueLabels.Add(logID, uniqueLabels.Count.ToString());
            }

            return "[" + uniqueLabels[logID] + "] ";
        }
        else {
            return "";
        }
    }
}
