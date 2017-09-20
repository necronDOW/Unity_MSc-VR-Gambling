using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 100.0f))
            {
                BettingMachineBehaviour bmb = info.collider.GetComponentInParent<BettingMachineBehaviour>();

                if (bmb)
                    bmb.ProjectToScreen(info.point, info.collider.gameObject);
            }
        }
    }
}
