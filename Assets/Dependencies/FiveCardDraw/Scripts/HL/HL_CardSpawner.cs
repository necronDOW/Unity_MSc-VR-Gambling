using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_CardSpawner : MonoBehaviour
{
    public Object cardPrefab;
    public float spawnHeight = 0.5f;
    public float spawnYVarience = 0.0f;
    public float spawnXZVarience = 0.0f;
    public float spawnYRotationVarience = 0.0f;
    public int spawnedCount { get { return transform.childCount; } }
    public int lastCardValue { get; private set; }

    private void OnEnable()
    {
        ResetSpawner(true);
    }

    private void OnDisable()
    {
        foreach (Transform t in transform)
            DestroyImmediate(t.gameObject);
    }

    public void ResetSpawner(bool spawnNewCard)
    {
        foreach (Transform t in transform)
            DestroyImmediate(t.gameObject);

        lastCardValue = 0;

        if (spawnNewCard)
            SpawnCard(true);
    }

    private int CalculateCardValue(bool higher)
    {
        if ((higher && lastCardValue + 1 >= FCD_Deck.valueCount) || (!higher && lastCardValue == 0))
            higher = !higher;

        int minRange = higher ? lastCardValue + 1 : 0;
        int maxRange = higher ? FCD_Deck.valueCount : lastCardValue;

        return lastCardValue = (Random.Range(minRange, maxRange * 1000) / 1000);
    }

    public int SpawnCard(bool higher)
    {
        if (cardPrefab) {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnXZVarience, spawnXZVarience),
                spawnHeight, Random.Range(-spawnXZVarience, spawnXZVarience));
            Quaternion spawnRotation = Quaternion.Euler(0.0f,
                Random.Range(-spawnYRotationVarience, spawnYRotationVarience), 0.0f);

            GameObject spawnedCard = (GameObject)Instantiate(cardPrefab, transform);
            spawnedCard.transform.localPosition = spawnPosition;
            spawnedCard.transform.localRotation = spawnRotation;

            HL_CardBehaviour cardBehaviour = spawnedCard.GetComponent<HL_CardBehaviour>();
            int cardValue = CalculateCardValue(higher);
            if (cardBehaviour)
                cardBehaviour.FlipCard(cardValue); // Take from a deck...?

            DespawnOverflow();
            return cardValue;
        }

        return -1;
    }

    private void DespawnOverflow()
    {
        if (transform.childCount > 3)
            Destroy(transform.GetChild(0).gameObject);
    }
}
