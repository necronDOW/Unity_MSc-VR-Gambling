using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_ButtonGroup : MonoBehaviour
{
    [SerializeField] FCD_PhysicalButton[] group;

    public void SetActive(bool value)
    {
        for (int i = 0; i < group.Length; i++)
            group[i].SetActive(value);
    }
}
