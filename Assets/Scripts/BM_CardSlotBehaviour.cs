using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BM_CardSlotBehaviour : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 1.0f;
    private BettingMachineBehaviour bm;
    private Light cardLight;
    private float maxLightIntensity, pulseChange;

    private void Start()
    {
        bm = GetComponentInParent<BettingMachineBehaviour>();
        cardLight = GetComponent<Light>();

        maxLightIntensity = cardLight.intensity;
        cardLight.intensity = 0.0f;
        pulseChange = -1.0f;
    }
    
    private void Update()
    {
        if (!bm.insertedCard)
        {
            cardLight.intensity += Time.deltaTime * pulseSpeed * pulseChange;

            if (cardLight.intensity > maxLightIntensity || cardLight.intensity <= 0.0f)
                pulseChange *= -1.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CardBehaviour cb = other.GetComponent<CardBehaviour>();
        if (cb && !bm.insertedCard)
            CardInserted(cb);
    }

    private void OnTriggerExit(Collider other)
    {
        if (bm.insertedCard && other.gameObject == bm.insertedCard.gameObject)
            CardRemoved();
    }

    private void CardInserted(CardBehaviour cb)
    {
        cb.transform.position = transform.position;
        cb.transform.rotation = transform.rotation;
        cb.GetComponent<Rigidbody>().isKinematic = true;
        
        cardLight.intensity = maxLightIntensity;
        if (pulseChange > 0)
            pulseChange *= -1.0f;

        bm.insertedCard = cb;
        bm.ToggleOn();
    }

    private void CardRemoved()
    {
        bm.insertedCard.GetComponent<Rigidbody>().isKinematic = false;
        bm.insertedCard = null;
        bm.ToggleOn();
    }
}