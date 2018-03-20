using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInteractionPoint : MonoBehaviour
{
    public bool showGizmos = true;

    private void OnDrawGizmos()
    {
        if (showGizmos) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, transform.localScale.x / 2.0f);

            //if (transform.parent && transform.parent.GetComponent<SteamVR_TrackedObject>()) {
            //    Gizmos.DrawLine(transform.position, transform.parent.position);
            //}
        }
    }
}
