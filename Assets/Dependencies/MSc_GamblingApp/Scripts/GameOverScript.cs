using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public PlayerData playerData;
    public Text uiDisplayState;
    public Text uiDisplayBalance;
    public Text uiDisplayStreak;

    private CameraFade cameraFade;

    private void Awake()
    {
        ChangeOutlinedText(uiDisplayState, playerData.cashedOut ? "Cashed Out!" : "Game Over!");
        ChangeOutlinedText(uiDisplayBalance, playerData.balance.ToString("0.00"), true);

        if (playerData.maxDiceStreak >= 0)
            ChangeOutlinedText(uiDisplayStreak, playerData.maxDiceStreak.ToString(), true);
        else uiDisplayStreak.gameObject.SetActive(false);

        cameraFade = Camera.main.GetComponent<CameraFade>();
    }

    public void Quit()
    {
        if (cameraFade && !cameraFade.ready)
            return;

        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        playerData.ResetAndSave();
    }

    private void ChangeOutlinedText(Text parent, string newText, bool concatinate = false)
    {
        if (parent)
        {
            string text = (concatinate ? parent.text : "") + newText;
            Text[] allText = parent.GetComponentsInChildren<Text>();

            for (int i = 0; i < allText.Length; i++)
                allText[i].text = text;
        }
    }
}
