using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCD_RiggingTools;

class FCD_Deck
{
    public const int valueCount = 13;
    public const int suitCount = 4;
    public const int totalCardCount = valueCount * suitCount;
    public const int wildcardCount = 0;
    public const int firstFaceIndex = 9;
    private const int lastFaceIndex = firstFaceIndex + 2;
    public const int firstRoyalsIndex = valueCount - 5;
    public static int maxSize { get { return (valueCount * suitCount) + wildcardCount; } }

    private static readonly string[] translatedValues = new string[13] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    private static readonly char[] translatedSuits = new char[4] { 'C', 'D', 'H', 'S' };
    public static string TranslateValue(int value)
    {
        if (value < 0 || value >= totalCardCount) {
            return "!(" + value.ToString() + ")";
        }

        return translatedValues[value % 13] + translatedSuits[value / 13];
    }
    
    private static List<FCD_Deck> _instances = new List<FCD_Deck>();
    public static FCD_Deck GetInstance(ref int index)
    {
        if (index < 0) {
            _instances.Add(new FCD_Deck());
            index = _instances.Count - 1;
            return _instances[index];
        }

        Extensions.Clamp(index, 0, _instances.Count - 1);
        return _instances[index];
    }

    private List<int> availableCards;
    private List<int> availableFaceCards;
    private Random rnd;

    private FCD_Deck()
    {
        availableCards = new List<int>((valueCount * suitCount) + wildcardCount);
        availableFaceCards = new List<int>(suitCount * (valueCount - firstFaceIndex));
        rnd = new Random();

        Reset();
    }

    public int DrawCard(int value)
    {
        if (availableCards.Remove(value)) {
            availableFaceCards.Remove(value);
            return value;
        }

        return -1;
    }

    public int DrawCardFromSimplified(int simplifiedValue)
    {
        rnd = new Random();
        int randomStartingIndex = rnd.Next(0, suitCount) * valueCount;

        for (int i = 0; i < suitCount; i++) {
            int newCard = DrawCard((randomStartingIndex + (valueCount * i) + simplifiedValue) % maxSize);
            if (newCard != -1)
                return newCard;
        }

        return -1;
    }

    private int GenerateRandomCard(List<int> drawFrom, params int[] simplifiedExceptions)
    {
        int randomIndex = rnd.Next(0, drawFrom.Count);
        
        for (int i = 0; i < simplifiedExceptions.Length; i++) {
            if (SimplifyValue(drawFrom[randomIndex]) == simplifiedExceptions[i]) {
                return GenerateRandomCard(drawFrom, simplifiedExceptions);
            }
        }

        return drawFrom[randomIndex];
    }

    public int DrawRandomCard()
    {
        int value = availableCards[rnd.Next(0, availableCards.Count)];

        availableCards.Remove(value);
        availableFaceCards.Remove(value);

        return value;
    }

    public int DrawRandomCard(params int[] simplifiedExceptions)
    {
        int value = GenerateRandomCard(availableCards, simplifiedExceptions);
        
        availableCards.Remove(value);
        availableFaceCards.Remove(value);

        return value;
    }

    public int DrawRandomCardInSuit(int suit)
    {
        int minValue = valueCount * suit;
        int maxValue = minValue + valueCount;

        while (true) {
            rnd = new Random();
            int card = DrawCard(rnd.Next(minValue, maxValue));
            if (card != -1)
                return card;
        }
    }

    public int DrawRandomFaceCard()
    {
        int value = availableFaceCards[rnd.Next(0, availableFaceCards.Count)];

        availableCards.Remove(value);
        availableFaceCards.Remove(value);

        return value;
    }

    public int DrawRandomFaceCard(params int[] simplifiedExceptions)
    {
        int value = GenerateRandomCard(availableFaceCards, simplifiedExceptions);

        availableCards.Remove(value);
        availableFaceCards.Remove(value);

        return value;
    }

    public bool InsertCard(int cardValue)
    {
        if (cardValue < 0 || cardValue >= maxSize || availableCards.Contains(cardValue))
            return false;
        else {
            for (int i = 0; i < availableCards.Count; i++) {
                if (availableCards[i] > cardValue) {
                    availableCards.Insert(i, cardValue);
                    break;
                }
            }

            if (ValueIsFace(cardValue)) {
                for (int i = 0; i < availableFaceCards.Count; i++) {
                    if (availableFaceCards[i] > cardValue) {
                        availableFaceCards.Insert(i, cardValue);
                        break;
                    }
                }
            }

            return true;
        }
    }

    public void Reset()
    {
        availableCards.Clear();
        availableFaceCards.Clear();

        for (int i = 0; i < availableCards.Capacity; i++) {
            availableCards.Add(i);;

            if (ValueIsFace(i))
                availableFaceCards.Add(i);
        }
    }

    public static bool ValueIsFace(int value)
    {
        return (SimplifyValue(value) >= firstFaceIndex && SimplifyValue(value) <= lastFaceIndex);
    }

    public static bool ValueIsRoyal(int value)
    {
        return (SimplifyValue(value) >= firstRoyalsIndex);
    }

    public static int SimplifyValue(int value)
    {
        return value % valueCount;
    }
}