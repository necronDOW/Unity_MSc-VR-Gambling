using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public bool ready { get; private set; }
    public float initialFadeIn = 0.0f;
    private Image blackoutImage;

    private void Awake()
    {
        Transform canvas = FindObjectOfType<Canvas>().transform;
        if (canvas)
        {
            blackoutImage = new GameObject().AddComponent<Image>();

            blackoutImage.name = "Fade";
            blackoutImage.transform.SetParent(canvas);
            blackoutImage.color = Color.clear;
            blackoutImage.raycastTarget = false;

            blackoutImage.rectTransform.anchorMin = Vector2.zero;
            blackoutImage.rectTransform.anchorMax = Vector2.one;
            blackoutImage.rectTransform.offsetMin = Vector2.zero;
            blackoutImage.rectTransform.offsetMax = Vector2.zero;
        }

        ready = true;
        FadeIn(initialFadeIn);
    }

    public void FadeIn(float duration)
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(Fade(duration, 1.25f, 0.0f));
        }
    }

    public void FadeOut(float duration)
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(Fade(duration, 0.0f, 1.25f));
        }
    }

    public void FadeOut(float duration, int nextScene)
    {
        if (ready)
        {
            FadeOut(duration);

            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene);
            async.allowSceneActivation = false;
            StartCoroutine(LoadSceneProgress(async));
        }
    }

    private IEnumerator LoadSceneProgress(AsyncOperation asyncScene)
    {
        while (asyncScene.progress < 0.9f || !ready)
            yield return null;

        asyncScene.allowSceneActivation = true;
    }

    private IEnumerator Fade(float duration, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime <= duration)
        {
            blackoutImage.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ready = true;
    }
}
