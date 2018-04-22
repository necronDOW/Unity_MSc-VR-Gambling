using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameMngr : MonoBehaviour
{
    [System.Serializable]
    public struct SceneNameFovPair {
        public string sceneName;
        public float fieldOfView;
    }

    public GameObject minigameInstancePrefab;
    [SerializeField] private float offsetOnLoad = 1000;
    [SerializeField] private float brightnessModifier = 1.0f;
    public SceneNameFovPair[] sceneNameFovPairs;

    private BettingMachineBehaviour[] bettingMachines;
    private Scene[] loadedMinigameScenes;
    //private Camera mainCamera;

    private void Awake()
    {
        bettingMachines = GameObject.FindGameObjectsWithTag("Betting Machine")
            .Select(x => x.GetComponent<BettingMachineBehaviour>()).ToArray();

        for (int i = 0; i < bettingMachines.Length; i++) {
            bettingMachines[i].minigameMngr = this;
            bettingMachines[i].machineIndex = i;
        }

        loadedMinigameScenes = new Scene[bettingMachines.Length];
    }

    private void Start()
    {
        //mainCamera = Camera.main;
    }

    public void LoadMinigame(int machineIndex, string sceneName)
    {
        UnloadMinigame(machineIndex);
        StartCoroutine(LoadSceneAsync(machineIndex, sceneName, SceneManager.sceneCount));
    }

    public void UnloadMinigame(int machineIndex)
    {
        if (loadedMinigameScenes[machineIndex].isLoaded)
            SceneManager.UnloadSceneAsync(loadedMinigameScenes[machineIndex]);
    }

    private IEnumerator LoadSceneAsync(int machineIndex, string name, int newSceneIndex)
    {
        if (name == "" || SceneManager.GetSceneByName(name) == null) {
            yield break;
        }

        bool isLoaded = false;

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!isLoaded) {
            if (op.isDone) {
                isLoaded = true;
                loadedMinigameScenes[machineIndex] = SceneManager.GetSceneAt(newSceneIndex);
                OnMinigameSceneLoaded(machineIndex);
            }

            yield return null;
        }
    }

    private void OnMinigameSceneLoaded(int machineIndex)
    {
        Camera gameSceneCamera = FindSceneCamera(loadedMinigameScenes[machineIndex]);
        gameSceneCamera.GetComponent<AudioListener>().enabled = false;

        bettingMachines[machineIndex].SetScreenFeed(gameSceneCamera, 1);

        //mainCamera.enabled = true;

        MoveSceneToHiddenLocation(machineIndex);
    }

    private void MoveSceneToHiddenLocation(int machineIndex)
    {
        GameObject[] rootObjects = loadedMinigameScenes[machineIndex].GetRootGameObjects();
        GameObject instance = Instantiate(minigameInstancePrefab);

        instance.transform.parent = rootObjects[0].transform;
        instance.transform.parent = null;

        for (int i = 0; i < rootObjects.Length; i++)
            rootObjects[i].transform.SetParent(instance.transform);

        float xOffset = -offsetOnLoad + (((offsetOnLoad * 2.0f) / bettingMachines.Length) * machineIndex);
        instance.transform.position = new Vector3(xOffset, -offsetOnLoad, offsetOnLoad);

        GameInstanceHandle gih = instance.GetComponent<GameInstanceHandle>();
        gih.brightnessModifier = brightnessModifier;
        gih.cameraFieldOfViewDefault = GetSceneDefaultFov(loadedMinigameScenes[machineIndex].name);
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

    private float GetSceneDefaultFov(string name)
    {
        if (sceneNameFovPairs != null) {
            for (int i = 0; i < sceneNameFovPairs.Length; i++) {
                if (sceneNameFovPairs[i].sceneName == name && sceneNameFovPairs[i].fieldOfView != 0.0f) {
                    return sceneNameFovPairs[i].fieldOfView;
                }
            }
        }

        return 60.0f;
    }
}
