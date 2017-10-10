using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioListener : MonoBehaviour
{
    private List<AudioSource> input = new List<AudioSource>();

    public void AddInput(AudioSource input)
    {
        this.input.Add(input);

        Transform originParent = input.transform.parent;
        input.transform.SetParent(transform);
        input.transform.localPosition = Vector3.zero;
    }

    public void PopulateInputsRelativeToCamera(Camera camera)
    {
        List<AudioSource> allSrc = new List<AudioSource>();
        GameObject[] rootObjects = camera.gameObject.scene.GetRootGameObjects();

        for (int i = 0; i < rootObjects.Length; i++)
        {
            List<GameObject> rootSrc = new List<GameObject>();
            FindAllObjectsWithComponent(rootObjects[i].transform, typeof(AudioSource), ref rootSrc);

            for (int j = 0; j < rootSrc.Count; j++)
            {
                AudioSource audioSrc = rootSrc[j].GetComponent<AudioSource>();
                float maxDistance = audioSrc.maxDistance;

                if (Vector3.Distance(rootSrc[j].transform.position, camera.transform.position) <= maxDistance)
                {
                    allSrc.Add(audioSrc);
                    AddInput(audioSrc);
                }

            }
        }
    }

    public void FindAllObjectsWithComponent(Transform root, System.Type type, ref List<GameObject> output)
    {
        if (root.GetComponent(type))
            output.Add(root.gameObject);

        for (int i = 0; i < root.transform.childCount; i++)
            FindAllObjectsWithComponent(root.GetChild(i), type, ref output);
    }
}