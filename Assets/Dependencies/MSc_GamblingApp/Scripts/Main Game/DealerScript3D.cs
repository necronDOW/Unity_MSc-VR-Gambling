using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScript3D : DealerScript
{
    private const float atlasStepX = 0.077f;
    private const float atlasStepY = 0.2f;
    
    public Material materialAtlas;

    protected override void Start()
    {
        base.Start();

        deck = new DeckScript3D(config.cardSceneOrderDirectory, materialAtlas, atlasStepX, atlasStepY);
        cardsInPlay = new CardScript3D[2];
    }
}