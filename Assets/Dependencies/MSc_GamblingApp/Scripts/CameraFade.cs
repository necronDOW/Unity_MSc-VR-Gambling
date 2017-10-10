using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

            blackoutImage.rectTransform.anchorMin = new Vector2(0, 0);
            blackoutImage.rectTransform.anchorMax = new Vector2(1, 1);

            blackoutImage.rectTransform.localPosition = Vector3.zero;
            blackoutImage.rectTransform.localRotation = Quaternion.identity;
            blackoutImage.rectTransform.localScale = Vector3.one;
        }

        ready = true;
        FadeIn(initialFadeIn);
    }
    
    public void FadeIn(float duration)
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(Fade(duration, 1.25f, 0.0f, null, 0));
        }
    }

    public void FadeOut(float duration) { FadeOut(duration, null, 0); }
    public void FadeOut(float duration, UnityAction<int> sceneSwitchInvoke, int nextScene)
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(Fade(duration, 0.0f, 1.25f, sceneSwitchInvoke, nextScene));
        }
    }

    private IEnumerator Fade(float duration, float startAlpha, float endAlpha, UnityAction<int> onComplete, int args0)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime <= duration)
        {
            blackoutImage.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ready = true;

        if (onComplete != null)
            onComplete.Invoke(args0);
    }
}
