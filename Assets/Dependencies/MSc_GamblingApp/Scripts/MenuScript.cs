using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public int gameSceneIndex = -1;
    public int tutorialSceneIndex = -1;

    private CameraFade cameraFade;

    private void Awake()
    {
        cameraFade = Camera.main.GetComponent<CameraFade>();
    }

    public void Play()
    {
        if (cameraFade && !cameraFade.ready)
            return;

        if (gameSceneIndex >= 0)
        {
            if (cameraFade)
                cameraFade.FadeOut(1.0f, gameSceneIndex);
            else UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneIndex);
        }
    }

    public void Tutorial()
    {
        if (cameraFade && !cameraFade.ready)
            return;

        if (tutorialSceneIndex >= 0)
        {
            if (cameraFade)
                cameraFade.FadeOut(1.0f, tutorialSceneIndex);
            else UnityEngine.SceneManagement.SceneManager.LoadScene(tutorialSceneIndex);
        }
    }

    public void Exit()
    {
        if (cameraFade && !cameraFade.ready)
            return;

        Application.Quit();
    }
}
