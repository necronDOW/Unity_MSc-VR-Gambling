using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BettingMachineBehaviour : MonoBehaviour
{
    [HideInInspector] public CardBehaviour insertedCard;
    [SerializeField] private Material screenBaseMaterial;
    [SerializeField] private GameObject[] screens;
    [SerializeField] private Camera[] feeds;

    private RenderTexture[] screenTextures;
    private Material[] screenMaterials;
    private bool isOn = true;
    private Scene loadedGameScene;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        screenTextures = new RenderTexture[screens.Length];
        screenMaterials = new Material[screens.Length];

        for (int i = 0; i < screens.Length; i++) {
            screenTextures[i] = new RenderTexture(1024, 1024, 24);

            screenMaterials[i] = new Material(screenBaseMaterial);
            screenMaterials[i].mainTexture = screenTextures[i];
            screenMaterials[i].color = isOn ? Color.white : Color.black;

            screens[i].GetComponent<MeshRenderer>().material = screenMaterials[i];

            SetScreenFeed(feeds[i], i);
        }

        LoadGameScene();
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
            feed.targetTexture = screenTextures[screenIndex];
    }

    public void ToggleOn()
    {
        isOn = !isOn;

        for (int i = 0; i < screens.Length; i++)
            screenMaterials[i].color = isOn ? Color.white : Color.black;
    }

    private void LoadGameScene()
    {
        if (loadedGameScene.isLoaded)
            SceneManager.UnloadSceneAsync(loadedGameScene);

        StartCoroutine(LoadSceneAsync("Test"));
    }
    
    private IEnumerator LoadSceneAsync(string name)
    {
        bool isLoaded = false;

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!isLoaded) {
            if (op.isDone) {
                isLoaded = true;
                loadedGameScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                OnGameSceneLoaded();
            }

            yield return null;
        }
    }

    private void OnGameSceneLoaded()
    {
        Camera gameSceneCamera = FindSceneCamera(loadedGameScene);
        gameSceneCamera.GetComponent<AudioListener>().enabled = false;
        SetScreenFeed(gameSceneCamera, 0);

        mainCamera.enabled = true;

        HideScene(loadedGameScene, gameSceneCamera);
    }

    private void HideScene(Scene s, Camera c)
    {
        GameObject[] rootObjects = s.GetRootGameObjects();
        GameObject gameSceneScalar = new GameObject("scalar");
        gameSceneScalar.transform.parent = rootObjects[0].transform;
        gameSceneScalar.transform.parent = null;

        for (int i = 0; i < rootObjects.Length; i++)
            rootObjects[i].transform.parent = gameSceneScalar.transform;

        gameSceneScalar.transform.localScale *= 0.001f;
        gameSceneScalar.transform.position += Vector3.down * 10.0f;

        c.nearClipPlane *= 0.001f;
        c.farClipPlane *= 0.001f;
    }

    private Camera FindSceneCamera(Scene s)
    {
        GameObject[] rootObjects = s.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++) {
            if (rootObjects[i].tag == "MainCamera")
                return rootObjects[i].GetComponent<Camera>();
        }

        return null;
    }
}
