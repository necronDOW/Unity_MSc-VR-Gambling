using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipEmitterScript : MonoBehaviour
{
    public Vector3 emitterOffset;
    public GameObject prefab;
    public float coinValue = 0.5f;

    private List<GameObject> coins = new List<GameObject>();
    private float leftOver = 0.0f;
    private float emitterRadius = 0.0f;

    private void Awake()
    {
        BoxCollider killZone = GetComponent<BoxCollider>();
        emitterRadius = Mathf.Min(killZone.size.x, killZone.size.z) / 3.0f;
    }
    
    public void Emit(float value)
    {
        int count = (int)((value + leftOver) / coinValue);

        for (int i = 0; i < count; i++)
        {
            float randomOffsetX = Random.Range(-emitterRadius, emitterRadius);
            float randomOffsetZ = Random.Range(-emitterRadius, emitterRadius);
            Vector3 radiusOffset = new Vector3(randomOffsetX, 0.0f, randomOffsetZ);

            coins.Add(Instantiate(prefab, transform.position + emitterOffset + radiusOffset, prefab.transform.rotation));
        }

        leftOver = value % coinValue;
    }

    public void Destroy(float value)
    {
        int count = (value > coins.Count * coinValue) ? coins.Count : (int)(value / coinValue);
        int startIndex = coins.Count - count;

        for (int i = startIndex; i < coins.Count; i++)
            Destroy(coins[i]);

        coins.RemoveRange(startIndex, count);
    }
}