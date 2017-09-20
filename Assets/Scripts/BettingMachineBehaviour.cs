using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BettingMachineBehaviour : MonoBehaviour
{
    [HideInInspector] public MinigameMngr minigameMngr;
    [HideInInspector] public int machineIndex = -1;
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
            screenTextures[i] = new RenderTexture(1024, 1024, 24);

            screenMaterials[i] = new Material(screenBaseMaterial);
            screenMaterials[i].mainTexture = screenTextures[i];
            screenMaterials[i].color = Color.white;

            screens[i].GetComponent<MeshRenderer>().material = screenMaterials[i];

            SetScreenFeed(feeds[i], i);
        }
        
        ToggleOn();
    }

    private void OnValidate()
    {
        if (feeds.Length != screens.Length)
        {
            Camera[] feedsTemp = feeds;

            feeds = new Camera[screens.Length];
            for (int i = 0; i < feeds.Length; i++)
                feeds[i] = feedsTemp[i];
        }
    }

    public void SetScreenFeed(Camera feed, int screenIndex)
    {
        if (!feed)
            return;

        if (screenIndex >= 0 && screenIndex < screens.Length)
        {
            feeds[screenIndex] = feed;
            feed.targetTexture = screenTextures[screenIndex];
        }
    }

    public void ToggleOn()
    {
        isOn = !isOn;

        if (isOn)
            minigameMngr.LoadMinigame(machineIndex, "gambling_3d");
        else minigameMngr.UnloadMinigame(machineIndex);
    }
}
