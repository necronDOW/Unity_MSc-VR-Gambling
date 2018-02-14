using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_CardBehaviour : MonoBehaviour
{
    private const float gfxOffsetX = (1.0f / 13.0f); // 1 being a full UV offset, divided by the number of values.
    private const float gfxOffsetY = -(1.0f / 5.0f); // 1 being a full UV offset, divided by the number of suits + 1 (jokers).

    public int CardValue { get; protected set; }

    public MeshRenderer faceRenderer;
    private Vector2 initialTextureOffset;
    private ParticleSystem shimmerParticles;

    protected virtual void Awake()
    {
        initialTextureOffset = faceRenderer.material.mainTextureOffset;
        shimmerParticles = GetComponentInChildren<ParticleSystem>();
    }

    public virtual void FlipCard(int cardValue)
    {
        this.CardValue = cardValue;
        UpdateCardImage();

        if (this.CardValue % FCD_Deck.valueCount == 12) {
            PlayShimmer(true);
        }
    }

    public void UpdateCardImage()
    {
        int suitOffset = CardValue / 13;
        int valueOffset = CardValue % 13;

        faceRenderer.material.mainTextureOffset = initialTextureOffset + new Vector2(gfxOffsetX * valueOffset, gfxOffsetY * suitOffset);
    }

    public void IncrementDelayedWalletReadyCount()
    {
        FCD_DealerBehaviour db = transform.parent.GetComponent<FCD_DealerBehaviour>();
        if (db && db.delayedWalletUpdater)
            db.delayedWalletUpdater.IncrementReady();
    }

    protected void PlayShimmer(bool value)
    {
        if (shimmerParticles != null)
        {
            if (value) shimmerParticles.Play();
            else shimmerParticles.Stop();
        }
    }
}
