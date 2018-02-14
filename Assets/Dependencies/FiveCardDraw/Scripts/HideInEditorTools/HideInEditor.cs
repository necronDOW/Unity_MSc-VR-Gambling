using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HideInEditor : MonoBehaviour
{
    private bool lastHidden = false;
    public bool hidden = false;
    private bool overidden = false;

    //private Renderer[] _renderers;
    //private Image[] _images;
    //private Text[] _texts;

    //private Renderer[] renderers { get { return SetArrayToComponents(ref _renderers); } }
    //private Image[] images { get { return SetArrayToComponents(ref _images); } }
    //private Text[] texts { get { return SetArrayToComponents(ref _texts); } }

    protected void Update()
    {
        if (!overidden && RequiresVisualsUpdate())
            RenderersEnabled(!(hidden && Application.isEditor) || Application.isPlaying);
    }

    protected void RenderersEnabled(bool value)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = value;

        foreach (Image i in GetComponentsInChildren<Image>())
            i.enabled = value;

        foreach (Text t in GetComponentsInChildren<Text>())
            t.enabled = value;
    }

    public void OverrideHidden(bool value)
    {
        overidden = true;
        RenderersEnabled(value);
    }

    protected T[] SetArrayToComponents<T>(ref T[] original)
    {
        if (original == null)
            original = GetComponentsInChildren<T>();
        return original;
    }

    private bool RequiresVisualsUpdate()
    {
        bool requiresUpdate = lastHidden != hidden;
        lastHidden = hidden;
        return requiresUpdate;
    }
}
