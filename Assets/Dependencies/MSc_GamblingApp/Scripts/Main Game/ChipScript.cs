using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipScript : MonoBehaviour
{
    public float maxLifeTime = 1.0f;
    public float interactionsWaitTime;

    private float lifeTime;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > maxLifeTime)
            KillInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhysicsKillZone" && rigidBody)
            StartCoroutine(KillInteractionCoroutine());
    }

    private IEnumerator KillInteractionCoroutine()
    {
        yield return new WaitForSeconds(interactionsWaitTime);
        KillInteraction();
    }

    private void KillInteraction()
    {
        Destroy(rigidBody);
        Destroy(this);
    }
}