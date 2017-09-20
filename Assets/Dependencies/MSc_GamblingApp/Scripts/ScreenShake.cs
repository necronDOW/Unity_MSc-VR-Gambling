using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : FXScript
{
    public float shakeDuration = 1.0f;
    public float shakeStrength = 0.0f;
    public float decreaseFactor = 1.0f;

    private float shake = 0.0f;
    private Transform target;
    private Vector3 startingPosition;

    private void Start()
    {
        target = Camera.main.transform;
        startingPosition = target.transform.localPosition;
    }

    private void Update()
    {
        if (shake > 0)
        {
            target.localPosition = startingPosition + Random.insideUnitSphere * shakeStrength;
            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shake = 0.0f;
            target.localPosition = startingPosition;
        }
    }

    public override void Trigger()
    {
        shake = shakeDuration;
    }
}
