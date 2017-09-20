using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBtnScript : PlayBtnScript3D
{
    public int areaIndex = -1;

    private DiceSpawnerScript spawner;

    protected override void Awake()
    {
        base.Awake();

        spawner = GetComponentInParent<DiceSpawnerScript>();
    }

    public override void Trigger()
    {
        base.Trigger();

        spawner.audioManager.PlaySelectSound();
        spawner.Evaluate(areaIndex);
    }
}