using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UINudge : MonoBehaviour
{
    private RectTransform rTransform;
    private Vector2 targetAnchoredPosition;

    private MaskableGraphic graphic;
    private Color targetColor;

    private Color tColor;

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        if (rTransform != null) {
            targetAnchoredPosition = rTransform.anchoredPosition;
        } else {
            Debug.LogError("'" + gameObject.name + "' cannot use the UINudge script as it is not a canvas element or is missing a RectTransform.");
            return;
        }

        graphic = GetUIGraphic();
        if (graphic != null) {
            targetColor = graphic.color;
        }

        tColor = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
        graphic.color = tColor;
    }

    private MaskableGraphic GetUIGraphic()
    {
        MaskableGraphic foundGraphic = GetComponent<Image>();
        if (foundGraphic != null) {
            return foundGraphic;
        }

        foundGraphic = GetComponent<Text>();
        return foundGraphic;
    }

    public void PlayNudge(float duration)
    {
        StopNudge();
        StartCoroutine(ColorLerp(duration * 0.05f, duration * 0.8f, duration * 0.15f));
        StartCoroutine(Nudge(5.0f, duration * 0.1f));
    }

    public void StopNudge()
    {
        StopAllCoroutines();

        tColor.a = 0.0f;
        graphic.color = tColor;
    }

    IEnumerator ColorLerp(float fadeInDuration, float fadeOutDuration, float fullyVisibleDuration)
    {
        float[] durations = { fadeInDuration, fullyVisibleDuration, fadeOutDuration };

        int phase = -1;
        while (phase < 3) {
            float t = 0.0f;

            if (++phase % 2 == 0) {
                while (t < durations[phase]) {
                    tColor.a = (phase == 0 ? t / durations[phase] : 1.0f - (t / durations[phase]));
                    graphic.color = tColor;

                    t += Time.deltaTime;
                    yield return null;
                }
            }
            else if (phase == 1) {
                yield return new WaitForSeconds(durations[1]);
            }
        }

        yield return null;
    }

    IEnumerator Nudge(float initialShake, float duration)
    {
        float t = 0.0f;
        float currentShake = initialShake;

        while (t < duration) {
            t += Time.deltaTime;
            currentShake -= Time.deltaTime * initialShake;

            float shakeX = Random.Range(-currentShake, currentShake);
            float shakeY = Random.Range(-currentShake, currentShake);
            rTransform.anchoredPosition = targetAnchoredPosition + new Vector2(shakeX, shakeY);

            yield return null;
        }

        rTransform.anchoredPosition = targetAnchoredPosition;
        yield return null;
    }
}
