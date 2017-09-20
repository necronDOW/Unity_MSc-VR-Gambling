using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript3D : CardScript
{
    protected override void ShowFace()
    {
        renderTarget.material = ((DeckScript3D)dealer.deck).RequestCardFace(value);
    }
}