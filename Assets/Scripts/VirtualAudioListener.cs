using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioListener : MonoBehaviour
{
    private List<AudioSource> input = new List<AudioSource>();

    public void AddInput(AudioSource input)
    {
        this.input.Add(input);
        input.transform.position = transform.position;
    }

    public void PopulateInputsRelativeToCamera(Camera camera)
    {
        List<AudioSource> allSrc = new List<AudioSource>();
        GameObject[] rootObjects = camera.scene.GetRootGameObjects();

        for (int i = 0; i < rootObjects.Length; i++) {
            AudioSource[] rootSrc = rootObjects[i].transform.GetComponentsInChildren<AudioSource>();

            for (int j = 0; j < rootSrc.Length; j++) {
                float maxDistance = rootSrc[j].maxDistance;
                
                if (Vector3.Distance(rootSrc[j].transform.position, camera.transform.position) <= maxDistance
                    && !allSrc.Contains(rootSrc[j])) {
                    allSrc.Add(rootSrc[j]);
                    AddInput(rootSrc[j]);
                }

            }
        }
    }
}
