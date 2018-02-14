using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FCD_ReturnsTableUI : MonoBehaviour
{
    public char valueMarker = '$';
    private Text textComponent;
    private string baseText;

    private void Awake()
    {
        textComponent = GetComponent<Text>();
        baseText = textComponent.text;
        UpdateUI(FCD_RiggingTools.Globals.returnsPercentages);
    }

    public void UpdateUI(int[] values, float division = 100)
    {
        float[] valuesAsFloats = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
            valuesAsFloats[i] = values[i] / division;
        UpdateUI(valuesAsFloats);
    }

    public void UpdateUI(float[] values)
    {
        string finalText = "";
        int lastIndexFound = 0;

        textComponent.text = baseText;

        for (int i = 0, j = 0; i < baseText.Length; i++) {
            if (baseText[i] == valueMarker) {
                string valueText = (values != null && j < values.Length) ? string.Format("{0:f2}", values[j++]) : "0.00";

                finalText += baseText.Substring(lastIndexFound, i - lastIndexFound) + valueText;
                lastIndexFound = i + 1;
            }
        }

        textComponent.text = finalText + baseText.Substring(lastIndexFound, baseText.Length - lastIndexFound);
    }
}
