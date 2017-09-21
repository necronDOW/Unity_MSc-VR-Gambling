﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BettingMachineBehaviour : MonoBehaviour
{
    [HideInInspector] public MinigameMngr minigameMngr;
    [HideInInspector] public int machineIndex = -1;
    [HideInInspector] public CardBehaviour insertedCard;

    [SerializeField] private Material screenBaseMaterial;
    [SerializeField] private BM_Screen[] screens;
    private bool isOn = false;

    private void Start()
    {
        for (int i = 0; i < screens.Length; i++)
            screens[i].Setup(this, i, 1024, 1024, screenBaseMaterial);
        
        ToggleOn();
    }

    public void ToggleOn()
    {
        isOn = !isOn;

        if (isOn)
            minigameMngr.LoadMinigame(machineIndex, "gambling_3d");
        else minigameMngr.UnloadMinigame(machineIndex);
    }

    public void SetScreenFeed(Camera feed, int index)
    {
        screens[index].SetFeed(feed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BM_Screen screen = collision.contacts[0].thisCollider.GetComponent<BM_Screen>();
        if (screen)
            screen.ProjectToMinigame(collision.contacts[0].point);
    }
}