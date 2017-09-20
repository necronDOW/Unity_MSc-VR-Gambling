using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenDebugger : MonoBehaviour
{
    public enum Type { Info, Warn, Error }
    
    private Text display;

    private void Awake()
    {
        display = GetComponent<Text>();
    }

    public void Log(Type messageType, string message)
    {
        display.text += "\n" + MessageString(messageType) + message;
    }

    private string MessageString(Type messageType)
    {
        switch (messageType)
        {
            case Type.Error:
                return "[ERROR] : ";
            case Type.Warn:
                return "[WARN] : ";
            case Type.Info: default:
                return "[INFO] : ";
        }
    }
}
