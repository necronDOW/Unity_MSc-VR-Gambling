using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BM_Screen : MonoBehaviour
{
    [SerializeField] private Camera feed;
    private BettingMachineBehaviour linkedMachine;
    private RenderTexture screenTexture;

    public void Setup(BettingMachineBehaviour linkedMachine, int screenIndex, int width, int height, Material baseMaterial)
    {
        this.linkedMachine = linkedMachine;

        screenTexture = new RenderTexture(width, height, 24);
        Material m = new Material(baseMaterial);
        m.mainTexture = screenTexture;
        m.color = Color.white;

        GetComponent<MeshRenderer>().material = m;
    }
    
    public void SetFeed(Camera target)
    {
        if (target)
        {
            feed = target;
            target.targetTexture = screenTexture;
        }
    }

    public void ProjectToMinigame(Vector3 hitPoint)
    {
        GameObject[] objects = HelperTools.Raycast(feed, ImpactPoint(hitPoint));
        for (int i = 0; i < objects.Length; i++)
        {
            EventHandle e = objects[i].GetComponent<EventHandle>();
            if (e)
                e.Trigger();
        }
    }

    private Vector3 ImpactPoint(Vector3 hitPoint)
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if (!bc)
            return Vector3.one * -1.0f;

        Vector3 pt1 = Vector3.right * bc.size.x * 0.5f;
        Vector3 pt2 = Vector3.forward * bc.size.z * 0.5f;
        Vector3 pt = pt1 + pt2;

        hitPoint = bc.transform.InverseTransformPoint(hitPoint);
        hitPoint += pt;
        pt *= 2.0f;

        float x = 1.0f - hitPoint.x / pt.x;
        float y = 1.0f - hitPoint.z / pt.z;

        return new Vector3(feed.pixelWidth * x, feed.pixelHeight * y);
    }
}
