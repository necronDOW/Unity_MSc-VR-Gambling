using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFX : FXScript
{
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    public override void Trigger()
    {
        for (int i = 0; i < particleSystems.Length; i++)
            particleSystems[i].Play();
    }
}
