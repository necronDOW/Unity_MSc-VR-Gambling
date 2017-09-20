using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript2D : CardScript
{
    protected override void ShowFace()
    {
        ((SpriteRenderer)renderTarget).sprite = ((DeckScript2D)dealer.deck).RequestCardFace(value);
    }
}