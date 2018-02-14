using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameInstanceHandle : MonoBehaviour
{
    public int instanceIndex = -1;
    public float brightnessModifier {
        set {
            if (sceneLights == null) {
                sceneLights = GetComponentsInChildren<Light>();
                sceneLightOriginalIntensity = new float[sceneLights.Length];

                for (int i = 0; i < sceneLights.Length; i++) {
                    sceneLightOriginalIntensity[i] = sceneLights[i].intensity;
                }
            }

            for (int i = 0; i < sceneLights.Length; i++) {
                sceneLights[i].intensity = sceneLightOriginalIntensity[i] * value;
            }
        }
    }

    private Light[] sceneLights;
    private float[] sceneLightOriginalIntensity;

    public void Start()
    {
        instanceIndex = IndexAmongstMatchingScenes();

        Transform eventSys = transform.Find("EventSystem");
        if (eventSys) {
            eventSys.GetComponent<EventSystem>().enabled = false;
            eventSys.GetComponent<StandaloneInputModule>().enabled = false;
        }
    }

    private int IndexAmongstMatchingScenes()
    {
        int instanceCount = 0;

        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex == gameObject.scene.buildIndex) {
                if (scene == gameObject.scene)
                    break;
                instanceCount++;
            }
        }

        return instanceCount;
    }
}
