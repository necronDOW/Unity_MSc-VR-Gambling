using System;
using System.Collections.Generic;
using System.Linq;
using FCD_RiggingTools;

class FCD_ProbabilitySystem
{
    public struct Probability
    {
        public HandType type { get; private set; }
        public int requiredCount { get; private set; }
        public ValueOccurence[] associatedCards { get; private set; }

        public Probability(HandType _type, int _requiredCount, ValueOccurence[] _associatedCards)
        {
            type = _type;
            requiredCount = _requiredCount;
            associatedCards = _associatedCards;
        }
        public Probability(Probability _other, HandType _type)
        {
            type = _type;
            requiredCount = _other.requiredCount;
            associatedCards = _other.associatedCards;
        }
    }
    
    // Check All Conditions
    public static Probability[] GetRequiredForAllCombinations(int[] baseHand, int handSizeLimit)
    {
        Probability[] probabilities = new Probability[Globals.returnsPercentages.Length];
        int maxAllowedCount = Extensions.Clamp(handSizeLimit - baseHand.Length, 0, handSizeLimit);

        ValueOccurenceList vol = new ValueOccurenceList(baseHand);

        // Independent probabilities (face pair and two pair).
        AddProbability(GetRequiredForFacePair, vol, maxAllowedCount, ref probabilities[(int)HandType.FacePair]);
        AddProbability(GetRequiredForTwoPair, vol, maxAllowedCount, ref probabilities[(int)HandType.TwoPair]);

        // If a three of a kind can be made ...
        if (AddProbability(GetRequiredForThreeOfAKind, vol, maxAllowedCount, ref probabilities[(int)HandType.ThreeOfAKind])) {
            // Check also for four of a kind and full house.
            AddProbability(GetRequiredForFourOfAKind, vol, maxAllowedCount, ref probabilities[(int)HandType.FourOfAKind]);
            AddProbability(GetRequiredForFullHouse, vol, maxAllowedCount, ref probabilities[(int)HandType.FullHouse]);
        }

        // If both a straight and flush can be made ...
        bool canStraight = AddProbability(GetRequiredForStraight, vol, maxAllowedCount, ref probabilities[(int)HandType.Straight]);
        bool canFlush = AddProbability(GetRequiredForFlush, vol, maxAllowedCount, ref probabilities[(int)HandType.Flush]);
        if (canStraight && canFlush) {
            // Check also for a straight-flush, and if that can be made ...
            if (AddProbability(GetRequiredForStraightFlush, vol, maxAllowedCount, ref probabilities[(int)HandType.StraightFlush])) {
                // Check also for a royal-flush.
                ValueOccurenceList royalsExclusiveVol = new ValueOccurenceList(baseHand, true);
                AddProbability(GetRequiredForRoyalFlush, royalsExclusiveVol, maxAllowedCount, ref probabilities[(int)HandType.RoyalFlush]);
            }
        }

        return probabilities;
    }

    private static bool AddProbability(Func<ValueOccurenceList, Probability> func, ValueOccurenceList vol, int maxAllowedCount, ref Probability probability)
    {
        probability = func(vol);
        if (probability.requiredCount <= maxAllowedCount)
            return true;

        return false;
    }

    private static int internal_GetRequiredFor(ValueOccurenceList vol, int count, int start, int end)
    {
        ValueOccurence vo;
        int requiredCount = internal_GetRequiredFor(vol, count, start, end, out vo);
        return requiredCount;
    }
    private static int internal_GetRequiredFor(ValueOccurenceList vol, int count, int start, int end, out ValueOccurence associatedCards)
    {
        associatedCards = vol.FindMostOccuringInSimplifiedRange(start, end);

        if (associatedCards != null)
            return (count - associatedCards.occurence).Clamp(0, count);
        else return count.Clamp(0, count);
    }

    public static Probability GetRequiredForNonFacePair(int[] baseHand)
    {
        ValueOccurenceList vol = new ValueOccurenceList(baseHand);
        return GetRequiredForNonFacePair(vol);
    }

    public static Probability GetRequiredForNonFacePair(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = new ValueOccurence[1];
        int requiredCount = internal_GetRequiredFor(vol, 2, 0, FCD_Deck.firstFaceIndex, out associatedCards[0]);
        return new Probability(HandType.FacePair, requiredCount, associatedCards);
    }

    // Face Pair
    //public static Probability GetRequiredForFacePair(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForFacePair(vol, out associatedCards);
    //}
    public static Probability GetRequiredForFacePair(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = new ValueOccurence[1];
        int requiredCount = internal_GetRequiredFor(vol, 2, FCD_Deck.firstFaceIndex, FCD_Deck.valueCount, out associatedCards[0]);
        return new Probability(HandType.FacePair, requiredCount, associatedCards);
    }

    // Two Pair
    //public static int GetRequiredForTwoPair(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForTwoPair(vol, out associatedCards);
    //}
    public static Probability GetRequiredForTwoPair(ValueOccurenceList vol)
    {
        int totalRequired = 0;
        List<ValueOccurence> voList = new List<ValueOccurence>();
        ValueOccurenceList volInstance = new ValueOccurenceList(vol);

        for (int i = 0; i < 2; i++)
        {
            ValueOccurence voTmp;
            totalRequired += internal_GetRequiredFor(volInstance, 2, 0, FCD_Deck.valueCount, out voTmp);
            voList.Add(voTmp);
            volInstance.Remove(voTmp);
        }

        ValueOccurence[] associatedCards = voList.ToArray();
        return new Probability(HandType.TwoPair, totalRequired, associatedCards);
    }

    // Three of a Kind
    //public static int GetRequiredForThreeOfAKind(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForThreeOfAKind(vol, out associatedCards);
    //}
    public static Probability GetRequiredForThreeOfAKind(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = new ValueOccurence[1];
        int requiredCount = internal_GetRequiredFor(vol, 3, 0, FCD_Deck.valueCount, out associatedCards[0]);
        return new Probability(HandType.ThreeOfAKind, requiredCount, associatedCards);
    }

    // Straight
    //public static int GetRequiredForStraight(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForStraight(vol, out associatedCards);
    //}
    public static Probability GetRequiredForStraight(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = vol.FindMostOccuringInRange(4);
        return new Probability(HandType.Straight, 5 - (associatedCards != null ? associatedCards.Length : 0), associatedCards);
    }

    // Flush
    //public static int GetRequiredForFlush(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForFlush(vol, out associatedCards);
    //}
    public static Probability GetRequiredForFlush(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = vol.FindMostOccuringSuited();
        return new Probability(HandType.Flush, (associatedCards != null ? 5 - associatedCards.Length : 5), associatedCards);
    }

    // Full House
    //public static int GetRequiredForFullHouse(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForFullHouse(vol, out associatedCards);
    //}
    public static Probability GetRequiredForFullHouse(ValueOccurenceList vol)
    {
        int totalRequired = 0;
        List<ValueOccurence> voList = new List<ValueOccurence>();
        ValueOccurenceList volInstance = new ValueOccurenceList(vol);

        for (int i = 0; i < 2; i++)
        {
            ValueOccurence voTmp;
            totalRequired += internal_GetRequiredFor(volInstance, 3 - i, 0, FCD_Deck.valueCount, out voTmp);
            voList.Add(voTmp);
            volInstance.Remove(voTmp);
        }

        ValueOccurence[] associatedCards = voList.ToArray();
        return new Probability(HandType.FullHouse, totalRequired, associatedCards);
    }

    // Four of a Kind
    //public static int GetRequiredForFourOfAKind(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForFourOfAKind(vol, out associatedCards);
    //}
    public static Probability GetRequiredForFourOfAKind(ValueOccurenceList vol)
    {
        ValueOccurence[] associatedCards = new ValueOccurence[1];
        int requiredCount = internal_GetRequiredFor(vol, 4, 0, FCD_Deck.valueCount, out associatedCards[0]);
        return new Probability(HandType.FourOfAKind, requiredCount, associatedCards);
    }

    // Straight Flush
    //public static int GetRequiredForStraightFlush(ValueOccurenceList vol)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForStraightFlush(vol, out associatedCards);
    //}
    public static Probability GetRequiredForStraightFlush(ValueOccurenceList vol)
    {
        Probability straightProbability = GetRequiredForStraight(vol);
        vol = new ValueOccurenceList(straightProbability.associatedCards);
        return new Probability(GetRequiredForFlush(vol), HandType.StraightFlush);
    }

    // Royal Flush
    //public static int GetRequiredForRoyalFlush(int[] baseHand)
    //{
    //    ValueOccurence[] associatedCards;
    //    return GetRequiredForRoyalFlush(baseHand, out associatedCards);
    //}
    public static Probability GetRequiredForRoyalFlush(ValueOccurenceList royalsExclusiveVol)
    {
        return new Probability(GetRequiredForStraightFlush(royalsExclusiveVol), HandType.RoyalFlush);
    }
}