using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public struct InternalScene_v2
{
    public string name;
    public Vector3 cameraLocalPosition;
    public Vector3 cameraEulerAngles;

    public GameObject container;
    public GameObject hudContainer;
    public MonoBehaviour[] targetBehaviour;
    public UnityEvent actionOnActive;
    
    public void SetActive(bool value, bool targetVisuals, bool targetInteractions)
    {
        if (container) {
            if (targetVisuals){
                SetActive<Renderer>(container, value);
            }

            if (targetInteractions){
                SetActive<Collider>(container, value);
            }
        }

        if (hudContainer){
            if (targetVisuals){
                SetActive<Text>(hudContainer, value);
                SetActive<Image>(hudContainer, value);
            }

            if (targetInteractions){
                SetActive<Button>(hudContainer, value);
            }
        }

        if (targetBehaviour != null){
            foreach (MonoBehaviour b in targetBehaviour)
                b.enabled = value;
        }

        if (value){
            actionOnActive.Invoke();
            SceneManager_v2.LogSceneSwitched(name);
        }
    }

    private void SetActive<T>(GameObject parent, bool value)
    {
        T[] allComponents = parent.GetComponentsInChildren<T>();

        if (typeof(T) == typeof(Renderer)) {
            for (int i = 0; i < allComponents.Length; i++)
                (allComponents[i] as Renderer).enabled = value;
        }
        else if (typeof(T) == typeof(Collider)) {
            for (int i = 0; i < allComponents.Length; i++)
                (allComponents[i] as Collider).enabled = value;
        }
        else if (typeof(T) == typeof(Text)) {
            for (int i = 0; i < allComponents.Length; i++)
                (allComponents[i] as Text).enabled = value;
        }
        else if (typeof(T) == typeof(Image)) {
            for (int i = 0; i < allComponents.Length; i++)
                (allComponents[i] as Image).enabled = value;
        }
        else if (typeof(T) == typeof(Button)) {
            for (int i = 0; i < allComponents.Length; i++)
                (allComponents[i] as Button).onClick.SetPersistentListenerState(i, value ? UnityEventCallState.RuntimeOnly : UnityEventCallState.Off);
        }
    }
}

public class SceneManager_v2 : MonoBehaviour
{
    public enum TransitionMode
    {
        Lerp,
        Fade,
        Instant
    }

    public enum DisableMode
    {
        PreTransition,
        PostTransition
    }

    [SerializeField] private Camera mainCamera;
    public int firstScene = 0;
    public InternalScene_v2[] scenes;
    public bool switchingScenes { get; private set; }
    public bool disableVisualElements = true;
    public bool disableInteractionElements = false;
    public const float lerpTransitionTime = 1.0f;
    
    private float switchTime = 0.0f;
    private int nextIndex = -1;
    private TransitionMode nextTransition;
    private DisableMode nextDisableMode;

    private void Update()
    {
        if (nextIndex != -1) {
            switchTime -= Time.deltaTime;

            if (switchTime < 0.0f) {
                Switch();
                switchTime = 0.0f;
                nextIndex = -1;
            }
        }
    }

    private void Awake()
    {
        if (firstScene < 0 || firstScene >= scenes.Length)
            firstScene = 0;
        SwitchScene(firstScene);
    }
    
    public void SwitchScene(int index, TransitionMode transition = TransitionMode.Instant, float waitForSeconds = 0.0f, DisableMode disableMode = DisableMode.PreTransition)
    {
        if (index < scenes.Length) {
            if (nextIndex == -1) {
                switchingScenes = true;
                switchTime = waitForSeconds;
                nextIndex = index;
                nextTransition = transition;
                nextDisableMode = disableMode;
            }
        }
    }

    private void Switch()
    {
        if (nextDisableMode == DisableMode.PreTransition) {
            DisableAllScenes();
        }

        switchingScenes = true;

        switch (nextTransition) {
            case TransitionMode.Lerp:
                StartCoroutine(QuadraticLerpCamera(scenes[nextIndex], lerpTransitionTime));
                break;
            case TransitionMode.Instant:
            default:
                InstantCamera(scenes[nextIndex]);
                break;
        }
    }

    private IEnumerator QuadraticLerpCamera(InternalScene_v2 scene, float duration)
    {
        float elapsedTime = 0.0f;
        Vector3 startPos = mainCamera.transform.localPosition;
        Vector3 startRot = mainCamera.transform.localEulerAngles;
        Vector3 dist = scene.cameraLocalPosition - startPos;
        Vector3 angDist = scene.cameraEulerAngles - startRot;

        while (elapsedTime <= duration) {
            mainCamera.transform.localPosition = EaseInOutQuad(elapsedTime, startPos, dist, duration);
            mainCamera.transform.localEulerAngles = EaseInOutQuad(elapsedTime, startRot, angDist, duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (nextDisableMode == DisableMode.PostTransition) {
            DisableAllScenes();
        }

        scene.SetActive(true, disableVisualElements, disableInteractionElements);
        switchingScenes = false;
        yield return null;
    }

    private Vector3 EaseInOutQuad(float t, Vector3 start, Vector3 dist, float d)
    {
        t /= d/2;
        if (t < 1.0f)
            return dist/2*t*t + start;

        t--;
        return -dist/2 * (t*(t-2) - 1) + start;
    }

    private void InstantCamera(InternalScene_v2 scene)
    {
        mainCamera.transform.localPosition = scene.cameraLocalPosition;
        mainCamera.transform.eulerAngles = scene.cameraEulerAngles;

        scene.SetActive(true, disableVisualElements, disableInteractionElements);
        switchingScenes = false;
    }

    private void DisableAllScenes(int exception = -1) {
        for (int i = 0; i < scenes.Length; i++) {
            if (i != exception)
                scenes[i].SetActive(false, disableVisualElements, disableInteractionElements);
        }
    }

    public static void LogSceneSwitched(string sceneName)
    {
        TimedDataLogger.Get().AddToLog(sceneName + " scene loaded", "scene loaded");
        TimedDataLogger.Get().Log("Scene Switched");
    }
}