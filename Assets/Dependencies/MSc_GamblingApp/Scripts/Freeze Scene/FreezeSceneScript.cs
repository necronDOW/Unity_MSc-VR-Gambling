using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSceneScript : MonoBehaviour
{
    public Configs config;
    public Animator uiAnimator;

    private float currentTime = 0.0f;

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= config.freezeSceneTime)
        {
            currentTime = 0.0f;
            GameObject.FindGameObjectWithTag("Scene Manager").GetComponent<SceneMngr>().SwitchScene(0, SceneMngr.TransitionMode.Instant, 1.5f);

            if (uiAnimator)
                uiAnimator.SetTrigger("Exit");
        }
    }
}
