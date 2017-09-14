using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettingMachineBehaviour : MonoBehaviour
{
    [HideInInspector] public CardBehaviour insertedCard;
    [SerializeField] private Material screenBaseMaterial;
    [SerializeField] private GameObject[] screens;
    [SerializeField] private Camera[] feeds;

    private RenderTexture[] screenTextures;
    private Material[] screenMaterials;
    private bool isOn = false;

    private void Start()
    {
        screenTextures = new RenderTexture[screens.Length];
        screenMaterials = new Material[screens.Length];

        for (int i = 0; i < screens.Length; i++) {
            screenTextures[i] = new RenderTexture(256, 256, 24);

            screenMaterials[i] = new Material(screenBaseMaterial);
            screenMaterials[i].mainTexture = screenTextures[i];
            screenMaterials[i].color = isOn ? Color.white : Color.black;

            screens[i].GetComponent<MeshRenderer>().material = screenMaterials[i];

            SetScreenFeed(feeds[i], i);
        }
    }

    public void SetScreenFeed(Camera feed, int screenIndex)
    {
        if (!feed)
            return;

        if (screenIndex >= 0 && screenIndex < screens.Length)
            feed.targetTexture = screenTextures[screenIndex];
    }

    public void ToggleOn()
    {
        isOn = !isOn;

        for (int i = 0; i < screens.Length; i++)
            screenMaterials[i].color = isOn ? Color.white : Color.black;
    }

    private void OnValidate()
    {
        if (feeds.Length != screens.Length) {
            Camera[] feedsTemp = feeds;

            feeds = new Camera[screens.Length];
            for (int i = 0; i < feeds.Length; i++)
                feeds[i] = feedsTemp[i];
        }
    }
}
