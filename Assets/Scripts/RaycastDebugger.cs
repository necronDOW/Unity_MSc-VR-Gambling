using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100.0f))
            {
                if (info.collider.GetComponent<BM_Screen>())
                    info.collider.GetComponent<BM_Screen>().ProjectToMinigame(info.point);
            }
        }
    }
}
