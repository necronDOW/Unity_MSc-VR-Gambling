using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScript2D : DealerScript
{
    protected override void Start()
    {
        base.Start();

        deck = new DeckScript2D(config.cardSceneOrderDirectory);
        cardsInPlay = new CardScript2D[2];
    }
}