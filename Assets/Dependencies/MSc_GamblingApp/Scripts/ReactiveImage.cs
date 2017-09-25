using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveImage : FXScript
{
    public MeshRenderer mRenderer;
    public float fadeDuration = 1.0f;
    private Color defaultColor, clickedColor;

    protected virtual void Awake()
    {
        if (!mRenderer)
            mRenderer = GetComponent<MeshRenderer>();
    }

    protected virtual void Start()
    {
        if (mRenderer)
        {
            defaultColor = mRenderer.material.color;
            clickedColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1.0f);
        }
    }

    public override void Trigger()
    {
        StartCoroutine(ChangeColor(mRenderer.material));
    }

    protected IEnumerator ChangeColor(Material material)
    {
        float t = 0.0f;
        material.color = clickedColor;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            material.color = Color.Lerp(clickedColor, defaultColor, t / fadeDuration);
            yield return null;
        }

        material.color = defaultColor;
    }

    private void OnDisable()
    {
        mRenderer.material.color = defaultColor;
    }

    private void OnApplicationQuit()
    {
        mRenderer.material.color = defaultColor;
    }
}
