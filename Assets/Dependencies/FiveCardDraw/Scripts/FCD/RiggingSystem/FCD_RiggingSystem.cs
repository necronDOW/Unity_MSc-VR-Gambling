using System;
using System.Collections.Generic;
using System.Linq;
using FCD_RiggingTools;

class FCD_RiggingSystem
{
    private static int[] EnsureCompleteProbability(FCD_Deck deckToDrawFrom, bool requiresFaceCards, ref FCD_ProbabilitySystem.Probability original, params int[] requiredCounts)
    {
        // This function dictates which card values will be assigned to an accompanying pair. E.g. if a two pair is required but only an Ace is held, it could assign any second value for the accompanying pair.
        // ** OPTIMIZATION : This function may be skippable is less than two requiredCounts are given as params. **

        List<int> addedCards = new List<int>();
        ValueOccurence[] voEnsured = new ValueOccurence[requiredCounts.Length]; // This represents the array of newly added values and includes the existing ones.
        ValueOccurence[] voArraySorted = original.associatedCards;              // The cards already associated with this particular set or sets.

        if (!voArraySorted.Contains(null)) {
            // Sort the associated cards set, based on the occurence of the simplified associated card, if no null values are found.
            try { voArraySorted = voArraySorted.OrderBy(x => x.occurence).ToArray(); }
            catch { }
        }
        Array.Sort(requiredCounts);

        for (int i = 0; i < voEnsured.Length; i++) {
            // This if statement determines whether or not a new value is required. The first condition would mean a new value is needed.
            if (i >= voArraySorted.Length || voArraySorted[i] == null) {
                // This loop creates a small array for existing simplified values. These values are disallowed when creating accompanying pairs or sets. E.g. to prevent a two-pair becoming a four of a kind.
                int[] simplifiedExceptions = new int[i];
                for (int j = 0; j < i; j++) {
                    simplifiedExceptions[j] = voEnsured[j].simplifiedValue;
                }

                // Add a new value whose simplified value does not match one of the existing sets. E.g. If an Ace is held, do not allow a second set of Aces.
                voEnsured[i] = new ValueOccurence(requiresFaceCards ? deckToDrawFrom.DrawRandomFaceCard(simplifiedExceptions) : deckToDrawFrom.DrawRandomCard(simplifiedExceptions));
                addedCards.Add(voEnsured[i].actualValues[0]);
                requiredCounts[i]--;
            }
            else {
                // This condition simply adds the existing value.
                voEnsured[i] = voArraySorted[i];
                requiredCounts[i] -= voEnsured[i].occurence;
            }
        }

        // Set the original probability object to the new one.
        original = new FCD_ProbabilitySystem.Probability(original.type, requiredCounts.Sum(), voEnsured);
        return addedCards.ToArray();
    }

    private static int[] internal_RigForPairs(FCD_Deck deckToDrawFrom, FCD_ProbabilitySystem.Probability probability, params int[] requiredCounts)
    {
        int index = 0;
        int[] addedCards = SmartArrayInit(probability.requiredCount);

        if (addedCards == null)
            return null;

        for (int i = 0; i < probability.associatedCards.Length; i++) {
            for (int j = 0; j < requiredCounts[i] - probability.associatedCards[i].occurence; j++) {
                int newCardValue = deckToDrawFrom.DrawCardFromSimplified(probability.associatedCards[i].simplifiedValue);

                if (newCardValue == -1 || index >= addedCards.Length)
                    return null;
                else addedCards[index++] = newCardValue;
            }
        }

        return addedCards;
    }

    private static int[] internal_RigForStraight(FCD_ProbabilitySystem.Probability probability, int minRange = 0, int maxRange = FCD_Deck.valueCount)
    {
        if (probability.associatedCards == null || probability.associatedCards[0].simplifiedValue < minRange || probability.associatedCards.Last().simplifiedValue > maxRange)
            return null;
        
        int diffRange = maxRange - minRange;
        int lastValue = probability.associatedCards[0].simplifiedValue;
        int requiredCount = probability.requiredCount;
        int indexer = 0;
        int[] addedCards = SmartArrayInit(requiredCount);

        if (addedCards == null)
            return null;

        for (int checkIndex = 0; indexer < addedCards.Length && checkIndex < probability.associatedCards.Length;) {
            int varience = probability.associatedCards[checkIndex].simplifiedValue - lastValue;

            for (int j = 1; j < varience; j++) {
                addedCards[indexer++] = lastValue + j;
                requiredCount--;
            }

            lastValue = probability.associatedCards[checkIndex++].simplifiedValue;
        }

        int topAdditions = (requiredCount > 1 ? requiredCount / 2 : requiredCount);
        topAdditions -= Extensions.Clamp((probability.associatedCards.Last().simplifiedValue + 1 + topAdditions) - maxRange, 0, diffRange);

        int bottomAdditions = requiredCount - topAdditions;
        int topCompensation = Extensions.Clamp((probability.associatedCards[0].simplifiedValue - bottomAdditions), -diffRange, 0);
        topCompensation -= Extensions.Clamp((minRange - (probability.associatedCards[0].simplifiedValue - bottomAdditions)), 0, int.MaxValue);
        topAdditions -= topCompensation;

        if (topCompensation != 0)
            bottomAdditions = Extensions.Clamp(bottomAdditions - topAdditions, 0, diffRange);

        for (int i = 0; i < topAdditions; i++)
            addedCards[indexer++] = probability.associatedCards.Last().simplifiedValue + 1 + i;

        for (int i = 0; i < bottomAdditions; i++)
            addedCards[indexer++] = probability.associatedCards[0].simplifiedValue - 1 - i;

        // Suit randomization
        Random rnd = new Random();
        int[] selectedSuits = new int[FCD_Deck.suitCount];
        for (int i = 0; i < addedCards.Length; i++) {
            int suitMultiplier = rnd.Next(0, selectedSuits.Length);
            addedCards[i] += FCD_Deck.valueCount * suitMultiplier;
            selectedSuits[suitMultiplier]++;
        }
        
        if (selectedSuits.Max() == probability.requiredCount) {
            addedCards[0] = (addedCards[0] + FCD_Deck.valueCount) % FCD_Deck.totalCardCount;
        }

        return addedCards;
    }

    public static int[] RigForHandType(ref HandType type, FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        switch (type)
        {
            case HandType.FacePair:
                return RigForFacePair(probability, deckToDrawFrom);

            case HandType.TwoPair:
                return RigForTwoPair(probability, deckToDrawFrom);

            case HandType.ThreeOfAKind:
                return RigForThreeOfAKind(probability, deckToDrawFrom);

            case HandType.FourOfAKind:
                return RigForFourOfAKind(probability, deckToDrawFrom);

            case HandType.FullHouse:
                return RigForFullHouse(probability, deckToDrawFrom);

            case HandType.Straight:
                return RigForStraight(probability, deckToDrawFrom);

            case HandType.Flush:
                return RigForFlush(probability, deckToDrawFrom);

            case HandType.StraightFlush:
                // If the maximum held is an Ace, it is impossible to provide less than a Royal Flush, so give a Flush instead.
                if (probability.associatedCards.Max(x => x.simplifiedValue) < FCD_Deck.valueCount - 1) {
                    return RigForStraightFlush(probability, deckToDrawFrom);
                }
                else {
                    // A Flush is given here as it is the previous most rewarding without deviating drastically from the balance curve.
                    type = HandType.Straight;
                    return RigForFlush(probability, deckToDrawFrom);
                }

            case HandType.RoyalFlush:
                return RigForRoyalFlush(probability, deckToDrawFrom);

            default:
                return null;
        }
    }

    public static int[] RigForFacePair(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedFromEnsure = EnsureCompleteProbability(deckToDrawFrom, true, ref probability, 2);
        int[] addedFromRig = internal_RigForPairs(deckToDrawFrom, probability, 2);
        return SmartConcat(addedFromEnsure, addedFromRig).ToArray();
    }

    public static int[] RigForTwoPair(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedFromEnsure = EnsureCompleteProbability(deckToDrawFrom, false, ref probability, 2, 2);
        int[] addedFromRig = internal_RigForPairs(deckToDrawFrom, probability, 2, 2);
        return SmartConcat(addedFromEnsure, addedFromRig).ToArray();
    }

    public static int[] RigForThreeOfAKind(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedFromEnsure = EnsureCompleteProbability(deckToDrawFrom, false, ref probability, 3);
        int[] addedFromRig = internal_RigForPairs(deckToDrawFrom, probability, 3);
        return SmartConcat(addedFromEnsure, addedFromRig).ToArray();
    }

    public static int[] RigForStraight(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedCards = internal_RigForStraight(probability);

        if (addedCards != null) {
            for (int i = 0; i < addedCards.Length; i++)
                addedCards[i] = deckToDrawFrom.DrawCard(addedCards[i]);
        }

        return addedCards;
    }

    public static int[] RigForFlush(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedCards = SmartArrayInit(probability.requiredCount);
        
        if (addedCards != null) {
            int flushSuit = probability.associatedCards[0].actualValues[0] / FCD_Deck.valueCount;

            // A temporary array is used to make sure that the flush is not drawing to a straight.
            int[] tempAdded = new int[addedCards.Length];
            for (int i = 0; i < tempAdded.Length; i++)
                tempAdded[i] = deckToDrawFrom.DrawRandomCardInSuit(flushSuit);

            tempAdded = tempAdded.OrderByDescending(x => x).ToArray();
            bool isStraight = true;
            for (int i = 1; i < tempAdded.Length; i++) {
                if (tempAdded[i-1] - tempAdded[i] != 1) {
                    isStraight = false;
                    break;
                }
            }

            if (isStraight) {
                tempAdded[0] = ((FCD_Deck.SimplifyValue(tempAdded[0]) + 1) % FCD_Deck.valueCount) * (FCD_Deck.valueCount * flushSuit);
            }

            for (int i = 0; i < addedCards.Length; i++)
                addedCards[i] = tempAdded[i];
        }

        return addedCards;
    }

    public static int[] RigForFullHouse(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedFromEnsure = EnsureCompleteProbability(deckToDrawFrom, false, ref probability, 2, 3);
        int[] addedFromRig = internal_RigForPairs(deckToDrawFrom, probability, 2, 3);
        return SmartConcat(addedFromEnsure, addedFromRig).ToArray();
    }

    public static int[] RigForFourOfAKind(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedFromEnsure = EnsureCompleteProbability(deckToDrawFrom, false, ref probability, 4);
        int[] addedFromRig = internal_RigForPairs(deckToDrawFrom, probability, 4);
        return SmartConcat(addedFromEnsure, addedFromRig).ToArray();
    }

    public static int[] RigForStraightFlush(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int maximumSimplified = probability.associatedCards.Max(x => x.simplifiedValue);
        // Cap the straight flush draw if the maximum simplified value is a face card. This prevents Royal Flushes.
        int[] addedCards = internal_RigForStraight(probability, 0, maximumSimplified >= FCD_Deck.firstFaceIndex ? maximumSimplified + 1 : 13);

        if (addedCards != null) {
            int suit = probability.associatedCards[0].actualValues[0] / FCD_Deck.valueCount;
            for (int i = 0; i < addedCards.Length; i++)
                addedCards[i] = deckToDrawFrom.DrawCard(FCD_Deck.SimplifyValue(addedCards[i]) + (FCD_Deck.valueCount * suit));
        }

        return addedCards;
    }

    public static int[] RigForRoyalFlush(FCD_ProbabilitySystem.Probability probability, FCD_Deck deckToDrawFrom)
    {
        int[] addedCards = internal_RigForStraight(probability, FCD_Deck.firstRoyalsIndex);
        
        if (addedCards != null) {
            int suit = probability.associatedCards[0].actualValues[0] / FCD_Deck.valueCount;
            for (int i = 0; i < addedCards.Length; i++)
                addedCards[i] = deckToDrawFrom.DrawCard(FCD_Deck.SimplifyValue(addedCards[i]) + (FCD_Deck.valueCount * suit));
        }

        return addedCards;
    }

    public static int[] RigForLoss(ValueOccurenceList vol, int requiredCount, FCD_Deck deckToDrawFrom)
    {
        int index = 0;
        Random rand = new Random();
        int[] output = new int[requiredCount];
        for (int i = 0; i < output.Length; i++)
            output[i] = -1;
        
        while (index < output.Length) {
            int randomValue = rand.Next(0, FCD_Deck.maxSize);
            
            // Pair check.
            if (MatchesSimplified(FCD_Deck.SimplifyValue(randomValue), vol) 
                || MatchesSimplified(FCD_Deck.SimplifyValue(randomValue), output))
                continue;

            if (index == output.Length - 1) {
                // Flush check.
                int existingSuit = (int)(vol.items[0].actualValues[0] / (float)FCD_Deck.valueCount);
                if (randomValue / FCD_Deck.valueCount == existingSuit) {
                    randomValue = (randomValue + FCD_Deck.valueCount) % FCD_Deck.totalCardCount;
                }

                // Straight check.
                // It is possible to generate a pair of <10 with this code, but impossible to generate a set of 3 or 4 because no pair could already exist.
                output[index] = randomValue;

                List<int> allSimplifiedValues = new List<int>();
                allSimplifiedValues.AddRange(output.Select(x => FCD_Deck.SimplifyValue(x)));
                allSimplifiedValues.AddRange(vol.items.Select(x => x.simplifiedValue));
                allSimplifiedValues.Sort();

                bool isStraight = true;
                for (int i = 1; i < allSimplifiedValues.Count; i++) {
                    if (allSimplifiedValues[i] - allSimplifiedValues[i - 1] != 1)
                        isStraight = false;
                }

                if (isStraight) {
                    int suitStart = (int)((float)randomValue / FCD_Deck.valueCount) * FCD_Deck.valueCount;

                    if (allSimplifiedValues[0] - 2 >= 0)
                        randomValue = allSimplifiedValues[0] - 2;
                    else randomValue = allSimplifiedValues[allSimplifiedValues.Count - 1] + 2;
                    
                    randomValue += suitStart;
                }

                output[index] = -1;
            }

            if (deckToDrawFrom.DrawCard(randomValue) != -1)
                output[index++] = randomValue;
        }

        return output;
    }

    private static bool MatchesSimplified(int simplifiedValue, ValueOccurenceList vol)
    {
        foreach (ValueOccurence vo in vol.items) {
            if (simplifiedValue == vo.simplifiedValue)
                return true;
        }

        return false;
    }

    private static bool MatchesSimplified(int simplifiedValue, int[] valueArray)
    {
        foreach (int v in valueArray) {
            if (v != -1) {
                int simplifiedV = v % FCD_Deck.valueCount;
                if (simplifiedValue == simplifiedV)
                    return true;
            }
        }

        return false;
    }

    private static int[] SmartConcat(int[] a, int[] b)
    {
        if (a == null)
            return b;
        else if (b == null)
            return a;
        else return a.Concat(b).ToArray();
    }

    private static int[] SmartArrayInit(int count)
    {
        if (count > 0)
            return new int[count];
        else return null;
    }
}
