using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript
{
    public readonly int jokerValue = 53;
    public readonly int suitValue = 13 * 1;

    private int[] cardOrder;
    private int currentIndex;

    public DeckScript(string orderDirectory)
    {
        string[] rawOrder = Helper.ReadFile(orderDirectory, 2);

        if (rawOrder != null)
        {
            cardOrder = new int[rawOrder.Length];

            for (int i = 0; i < cardOrder.Length; i++)
                cardOrder[i] = ParseValue(rawOrder[i]);
        }
    }

    public int GetNextCard()
    {
        if (currentIndex < cardOrder.Length)
            return cardOrder[currentIndex++];
        return -1;
    }

    private int ParseValue(string raw)
    {
        int value;
        
        if (!int.TryParse(raw, out value))
        {
            switch (raw[0])
            {
                case 'J': value = 11; break;
                case 'Q': value = 12; break;
                case 'K': value = 13; break;
                case 'A': value = 1; break;
                default: value = -1; break;
            }
        }

        return (value != -1) ? suitValue + value : jokerValue;
    }
}

public class DeckScriptGeneric<T> : DeckScript where T : Object
{
    protected T[] cardFaces;

    public DeckScriptGeneric(string orderDirectory)
        : base(orderDirectory) { }

    public T RequestCardFace(int value)
    {
        if (value < cardFaces.Length)
            return cardFaces[value - 1];
        return null;
    }
}

public class DeckScript2D : DeckScriptGeneric<Sprite>
{
    public DeckScript2D(string orderDirectory)
        : base(orderDirectory)
    {
        cardFaces = Resources.LoadAll<Sprite>("playing_cards");
    }
}

public class DeckScript3D : DeckScriptGeneric<Material>
{
    public DeckScript3D(string orderDirectory, Material atlas, float atlasStepX, float atlasStepY)
        : base(orderDirectory)
    {
        cardFaces = Helper.MaterialsFromExisting(atlas, atlasStepX, atlasStepY);
    }
}