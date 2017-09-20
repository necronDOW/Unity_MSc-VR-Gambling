using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InternalScene
{
    public Vector3 cameraLocalPosition;
    public Vector3 cameraEulerAngles;

    public GameObject container;
    public GameObject hudContainer;

    public void SetActive(bool value)
    {
        container.SetActive(value);

        if (hudContainer)
            hudContainer.SetActive(value);
    }
}

public class SceneMngr : MonoBehaviour
{
    public enum TransitionMode
    {
        Lerp,
        Fade,
        Instant
    }

    public Camera mainCamera;
    public int firstScene = 0;
    public InternalScene[] scenes;

    private float switchTime = 0.0f;
    private int nextIndex = -1;
    private TransitionMode nextTransition;

    private void Update()
    {
        if (nextIndex != -1)
        {
            switchTime -= Time.deltaTime;

            if (switchTime < 0.0f)
            {
                Switch(nextIndex, nextTransition);
                switchTime = 0.0f;
                nextIndex = -1;
            }
        }
    }

    private void Awake()
    {
        if (firstScene < 0 || firstScene >= scenes.Length)
            firstScene = 0;
        SwitchScene(firstScene, TransitionMode.Lerp);
    }

    public void SwitchScene(int index, TransitionMode transition, float waitForSeconds = 0.0f)
    {
        if (index < scenes.Length)
        {
            if (nextIndex == -1)
            {
                switchTime = waitForSeconds;
                nextIndex = index;
                nextTransition = transition;
            }
        }
    }

    private void Switch(int index, TransitionMode transition)
    {
        for (int i = 0; i < scenes.Length; i++)
            scenes[i].SetActive(false);

        switch (transition)
        {
            case TransitionMode.Lerp:
                StartCoroutine(QuadraticLerpCamera(scenes[index], 2.0f));
                break;
            case TransitionMode.Instant:
            default:
                InstantCamera(scenes[index]);
                break;

        }
    }

    private IEnumerator QuadraticLerpCamera(InternalScene scene, float duration)
    {
        float elapsedTime = 0.0f;
        Vector3 startPos = mainCamera.transform.localPosition;
        Vector3 startRot = mainCamera.transform.localEulerAngles;
        Vector3 dist = scene.cameraLocalPosition - startPos;
        Vector3 angDist = scene.cameraEulerAngles - startRot;

        while (elapsedTime <= duration)
        {
            mainCamera.transform.localPosition = EaseInOutQuad(elapsedTime, startPos, dist, duration);
            mainCamera.transform.localEulerAngles = EaseInOutQuad(elapsedTime, startRot, angDist, duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scene.SetActive(true);
    }

    private Vector3 EaseInOutQuad(float t, Vector3 start, Vector3 dist, float d)
    {
        t /= d/2;
        if (t < 1.0f)
            return dist/2*t*t + start;

        t--;
        return -dist/2 * (t*(t-2) - 1) + start;
    }

    private void InstantCamera(InternalScene scene)
    {
        mainCamera.transform.localPosition = scene.cameraLocalPosition;
        mainCamera.transform.eulerAngles = scene.cameraEulerAngles;

        scene.SetActive(true);
    }
}