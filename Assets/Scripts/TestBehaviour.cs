using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    public float maxXOffset = 1.0f;
    public float speed = 1.0f;

    private Vector3 minPos, maxPos;
    private bool direction = true;

    private void Start()
    {
        minPos = transform.localPosition - new Vector3(maxXOffset, transform.localPosition.y, transform.localPosition.z);
        maxPos = transform.localPosition + new Vector3(maxXOffset, transform.localPosition.y, transform.localPosition.z);
    }

    float t = 0.5f;
    private void Update()
    {
        t += speed * Time.deltaTime * (direction ? 1.0f : -1.0f);
        transform.localPosition = Vector3.Lerp(minPos * transform.lossyScale.x, maxPos * transform.lossyScale.x, t);

        if (t > 1.0f || t < 0.0f)
            direction = !direction;
    }
}
