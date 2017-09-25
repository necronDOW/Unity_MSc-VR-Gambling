using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBtnScript3D : ReactiveImage
{
    [SerializeField] private DealerScript3D dealer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, 100.0f))
            {
                if (hitInfo.collider.gameObject == gameObject && hitInfo.collider.isTrigger)
                    Trigger();
            }
        }
    }

    public override void Trigger()
    {
        base.Trigger();

        if (dealer)
            dealer.GetComponent<DealerScript3D>().Deal();
    }
}
