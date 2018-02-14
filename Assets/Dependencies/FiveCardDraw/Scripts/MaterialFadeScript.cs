using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFadeScript : MonoBehaviour
{
    private Material materialInstance;
    private Color originalColor;

    private void Awake()
    {
        materialInstance = GetComponent<MeshRenderer>().material;
        originalColor = materialInstance.color;
    }

    public void ResetColor()
    {
        materialInstance.color = originalColor;
    }

    public void BeginFadeIn(float duration)
    {
        StartCoroutine(FadeToColor(materialInstance, originalColor, duration));
    }

    public void BeginFadeOut(float duration, float targetBrightnessAsPercentage)
    {
        Color targetColor = originalColor * targetBrightnessAsPercentage;
        targetColor.a = originalColor.a;
        StartCoroutine(FadeToColor(materialInstance, targetColor, duration));
    }

    public static IEnumerator FadeToColor(Material targetMaterial, Color targetColor, float duration)
    {
        if (duration > 0.0f) {
            float t = Time.deltaTime;
            Color colorDiff = (targetColor - targetMaterial.color) / duration;

            while (duration >= 0.0f)
            {
                targetMaterial.color += colorDiff * Time.deltaTime;
                duration -= Time.deltaTime;
                yield return null;
            }
        }

        targetMaterial.color = targetColor;
    }
}
