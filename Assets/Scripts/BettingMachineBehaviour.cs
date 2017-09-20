using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
        if (feeds.Length != screens.Length) {
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

        if (screenIndex >= 0 && screenIndex < screens.Length) {
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

    public void ProjectToScreen(Vector3 hitPoint, GameObject screen)
    {
        int impactScreen = -1;
        for (int i = 0; i < screens.Length; i++)
        {
            if (screen == screens[i])
                impactScreen = i;
        }

        if (impactScreen == -1)
            return;

        BoxCollider bc = screens[impactScreen].GetComponent<BoxCollider>();
        Vector3 pt1 = Vector3.right * bc.size.x * 0.5f;
        Vector3 pt2 = Vector3.forward * bc.size.z * 0.5f;
        Vector3 pt = pt1 + pt2;

        hitPoint = bc.transform.InverseTransformPoint(hitPoint);
        hitPoint += pt;
        pt *= 2.0f;

        float x = 1.0f - hitPoint.x / pt.x;
        float y = 1.0f - hitPoint.z / pt.z;
        Vector3 feedPixelPoint = new Vector3(feeds[impactScreen].pixelWidth * x, feeds[impactScreen].pixelHeight * y);

        GameObject[] objects = Raycast(feeds[impactScreen], feedPixelPoint);
        for (int i = 0; i < objects.Length; i++)
            Debug.Log(objects[i].name);
    }

    public GameObject[] Raycast(Camera camera, Vector3 pointerPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = pointerPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        List<GameObject> output = results.Select(x => x.gameObject).ToList();

        RaycastHit info;
        Ray ray = camera.ScreenPointToRay(pointerPosition);
        if (Physics.Raycast(ray, out info, 100.0f))
            output.Add(info.collider.gameObject);

        return output.ToArray();
    }
}