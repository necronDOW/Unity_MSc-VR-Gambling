using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioManager[] managers;

    private void Awake()
    {
        for (int i = 0; i < managers.Length; i++)
            managers[i].Init();
    }
}
