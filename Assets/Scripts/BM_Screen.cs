using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BM_Screen : MonoBehaviour
{
    private const float minInteractionsDistance = 0.1f;

    [SerializeField] private Camera feed;
    private BettingMachineBehaviour linkedMachine;
    private RenderTexture screenTexture;
    private List<GameObject> blockedInteractions;

    private void Update()
    {
        foreach (GameObject g in blockedInteractions) {
            RaycastHit info;
            if (Physics.Raycast(g.transform.position, transform.forward, out info, 5.0f)) {
                if (info.collider.gameObject == gameObject && info.distance > minInteractionsDistance) {
                    blockedInteractions.Remove(g);
                }
            }
        }
    }
    
    public void Setup(BettingMachineBehaviour linkedMachine, int screenIndex, int width, int height, Material baseMaterial)
    {
        this.linkedMachine = linkedMachine;

        screenTexture = new RenderTexture(width, height, 24);
        Material m = new Material(baseMaterial);
        m.mainTexture = screenTexture;
        m.color = Color.white;

        blockedInteractions = new List<GameObject>();

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

    public void AddToBlockedInteractions(Collision collisionInfo)
    {
        if (!blockedInteractions.Contains(collisionInfo.gameObject)) {
            blockedInteractions.Add(collisionInfo.gameObject);
        }
    }

    public bool InteractionIsBlocked(Collision collisionInfo)
    {
        foreach (GameObject g in blockedInteractions) {
            if (g == collisionInfo.gameObject) {
                return true;
            }
        }

        return false;
    }

    private Vector3 ImpactPoint(Vector3 hitPoint)
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if (!bc)
            return Vector3.one * -1.0f;

        Vector3 pt1 = Vector3.right * bc.size.x * 0.5f;
        Vector3 pt2 = Vector3.up * bc.size.y * 0.5f;
        Vector3 pt = pt1 + pt2;

        hitPoint = bc.transform.InverseTransformPoint(hitPoint);
        hitPoint += pt;
        pt *= 2.0f;

        Debug.DrawLine(pt, -pt, Color.blue, Mathf.Infinity);
        Debug.DrawLine(Vector3.zero, hitPoint, Color.red, Mathf.Infinity);

        float x = hitPoint.x / pt.x;
        float y = hitPoint.y / pt.y;

        return new Vector3(feed.pixelWidth * x, feed.pixelHeight * y);
    }
}
