using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelperTools
{
    public static GameObject[] Raycast(Camera camera, Vector3 pointerPosition)
    {
        List<GameObject> output = new List<GameObject>();
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = pointerPosition;
        EventSystem.current.RaycastAll(pointerData, results);

        for (int i = 0; i < results.Count; i++) {
            if (results[i].gameObject.GetComponent<Button>())
                output.Add(results[i].gameObject);
        }

        if (output.Count == 0) {
            RaycastHit info;
            Ray ray = camera.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(ray, out info, 100.0f))
                output.Add(info.collider.gameObject);
        }

        return output.ToArray();
    }
}
