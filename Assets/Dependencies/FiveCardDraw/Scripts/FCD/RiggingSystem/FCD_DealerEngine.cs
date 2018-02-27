﻿#define USE_CALCULATION_VOLATILITY

using System;
using System.Collections.Generic;
using FCD_RiggingTools;

class FCD_DealerEngine
{
    public class Hand
    {
        private int state;
        public bool[] heldIndices { get; private set; }
        private int[] handValues;
        public int size { get { return handValues.Length; } }
        public List<int> currentHand
        {
            get {
                List<int> heldValues = new List<int>();

                for (int i = 0; i < heldIndices.Length; i++) {
                    if (heldIndices[i])
                        heldValues.Add(handValues[i]);
                }

                return heldValues;
            }
        }
        public int[] fullHand { get { return handValues; } }

        public Hand(int[] values)
        {
            heldIndices = new bool[values.Length];
            handValues = values;
            state = 0;
        }

        public int this[int key]
        {
            get { return handValues[key]; }
            set { handValues[key] = value; }
        }

        public void ToggleHold(int index)
        {
            if (state == 0 && index >= 0 && index < heldIndices.Length) {
                heldIndices[index] = !heldIndices[index];

                string heldStatusPrefix = heldIndices[index] ? "FCD Hold" : "FCD Discard";
                TimedDataLogger.Get().AddToLog(heldStatusPrefix + " Card (index=" + index + ", value=" + FCD_Deck.TranslateValue(handValues[index]) + ")", "hold");
            }
        }

        public void Finalize(FCD_Deck target)
        {
            if (state == 0) {
                for (int i = 0; i < heldIndices.Length; i++) {
                    if (!heldIndices[i]) {
                        if (target.InsertCard(handValues[i]))
                            handValues[i] = -1;
                    }
                    else
                        target.DrawCard(handValues[i]);
                }
            }

            state++;
        }

        public void Merge(int[] newValues)
        {
            if (state == 1) {
                for (int i = 0, j = 0; i < handValues.Length && j < newValues.Length; i++) {
                    if (handValues[i] == -1) {
                        handValues[i] = newValues[j++];
                    }
                }
            }
        }

        public void Lock()
        {
            for (int i = 0; i < heldIndices.Length; i++)
                heldIndices[i] = true;

            state = 2;
        }
    }

    private static List<FCD_DealerEngine> _instances = new List<FCD_DealerEngine>();
    public static FCD_DealerEngine GetInstance(ref int index)
    {
        if (index < 0)
        {
            _instances.Add(new FCD_DealerEngine());
            index = _instances.Count - 1;
            return _instances[index];
        }

        Extensions.Clamp(index, 0, _instances.Count - 1);
        return _instances[index];
    }
    
    public Hand hand { get; private set; }
    public int turn { get; private set; }
    public List<int> winningValues { get; private set; }
    public bool handWasWin { get; private set; }

    public int deckInstance = -1;
    private FCD_DealerAssistant assistant;
    public FCD_WalletScript walletScript;

    private FCD_DealerEngine()
    {
        assistant = FCD_DealerAssistant.Instance;
        assistant.InitializeWinSequence(Globals.winSequencePath);
        assistant.InitializeLoseSequence(Globals.loseSequencePath);
    }

    public bool DrawNewHand()
    {
        if (!canDrawNewHand)
            return false;

        if (NewHandFromArray(assistant.GetCardSet(walletScript.wallet - Globals.balanceCurve[turn]))) {
            DataLogger.Get().AddToLog("Balance", string.Format("£{0:f2}", walletScript.wallet));
            DataLogger.Get().AddToLog("Target", string.Format("£{0:f2}", Globals.balanceCurve[turn]));

            walletScript.wallet -= Globals.betAmount;
            walletScript.UpdateUI();
            return true;
        }

        return false;
    }

    public bool canDrawNewHand {
        get {
            return turn < Globals.balanceCurve.Length && walletScript.wallet - Globals.betAmount >= 0;
        }
    }

    private bool NewHandFromArray(int[] hand)
    {
        this.hand = null;

        // Changed from "DrawCard" to "DrawCardFromSimplified".
        for (int i = 0; i < hand.Length; i++) {
            if ((hand[i] = FCD_Deck.GetInstance(ref deckInstance).DrawCardFromSimplified(hand[i])) == -1)
                return false;
        }

        this.hand = new Hand(hand);
        return true;
    }

    public bool ToggleHold(int cardIndex)
    {
        if (cardIndex >= 0 && cardIndex < hand.size) {
            hand.ToggleHold(cardIndex);
            return true;
        }

        return false;
    }
    //public bool DiscardValue(int cardValue)
    //{
    //    for (int i = 0; i < currentHand.Count; i++) {
    //        if (currentHand[i] == cardValue)
    //            return DiscardIndex(i);
    //    }

    //    return false;
    //}
    
    public void CompleteRiggedHand()
    {
        winningValues = null;
        hand.Finalize(FCD_Deck.GetInstance(ref deckInstance));

        if (turn >= Globals.balanceCurve.Length || hand == null)
            return;

        float balanceVariance = walletScript.wallet - Globals.balanceCurve[turn];

        // Load probable indices.
        List<int> probableIndices = new List<int>();
        FCD_ProbabilitySystem.Probability[] probabilities = FCD_ProbabilitySystem.GetRequiredForAllCombinations(hand.currentHand.ToArray(), 5);
        for (int i = 0; i < probabilities.Length; i++) {
            if (probabilities[i].requiredCount <= (5 - hand.currentHand.Count) && probabilities[i].associatedCards != null) {
                probableIndices.Add(i);
            }
        }

        // Check if a completed result already exists.
        bool alreadyHasResult = false;
        for (int i = 0; i < probableIndices.Count; i++) {
            if (probabilities[probableIndices[i]].requiredCount == 0) {
                alreadyHasResult = true;
                break;
            }
        }

        //lastHandWasWin = FCD_DealerAssistant.GetRandomizedWin(balanceVariance, Globals.betAmount);
        handWasWin = FCD_DealerAssistant.GetWin(balanceVariance) || alreadyHasResult;
        int[] addedCards = null;
        int selectedIndex = -1;

        if (handWasWin) {
            #region old_filter
            // IMPORTANT: This code filters out probably indices which would require the removal of cards to achieve (e.g. if the player holds a two pair, it won't result in anything less).
            //int highestProbabilityWithZeroRequired = -1;
            //for (int i = 0; i < probableIndices.Count; i++) {
            //    UnityEngine.Debug.Log("Required for " + (HandType)probableIndices[i] + ": " + probabilities[probableIndices[i]].requiredCount);
            //    if (probabilities[probableIndices[i]].requiredCount == 0) {
            //        highestProbabilityWithZeroRequired = i;
            //    }
            //}

            //if (highestProbabilityWithZeroRequired > 0) {
            //    UnityEngine.Debug.Log("Removed probabilities: 0->" + highestProbabilityWithZeroRequired + ".");
            //    probableIndices.RemoveRange(0, highestProbabilityWithZeroRequired);
            //}
            #endregion
            #region filter
            // IMPORTANT: This code filters out FacePairs as a probability if a Pair of non-face cards exists.
            if (probableIndices.Count >= 2 && (probableIndices[0] == 0 && probableIndices[1] == 1)) {
                int requiredForTwoPair = probabilities[probableIndices[1]].requiredCount;
                int requiredForNonFacePair = FCD_ProbabilitySystem.GetRequiredForNonFacePair(hand.currentHand.ToArray()).requiredCount;

                if (requiredForTwoPair == 0 || requiredForNonFacePair == 0) {
                    probableIndices.Remove(0);
                }
            }
            #endregion

            if (probableIndices.Count != 0) {
                selectedIndex = ClosestPercentageGainIndex(Math.Abs(balanceVariance), probableIndices);

#if USE_CALCULATION_VOLATILITY
                selectedIndex = RandomizeIndexBelowTarget(probableIndices, selectedIndex); /* This line causes the selected index to be a range from 0 to the 
                                                                                              target index (i.e. if 2 is desired, rand(0, 2)). */
#endif

                addedCards = FCD_RiggingSystem.RigForHandType((HandType)selectedIndex, probabilities[selectedIndex], FCD_Deck.GetInstance(ref deckInstance));

                float winnings = Globals.betAmount * (Globals.returnsPercentages[selectedIndex] * 0.01f);
                
                if (FCD_DealerBehaviour.isDebugSimulation) {
                    walletScript.AddDirect(winnings);
                }
                else {
                    walletScript.wallet += winnings;
                }

                DataLogger.Get().AddToLog("Outcome", "" + (HandType)selectedIndex);
                DataLogger.Get().AddToLog("Profit", string.Format("£{0:f2}", winnings - Globals.betAmount));

                foreach (ValueOccurence vo in probabilities[selectedIndex].associatedCards) {
                    if (vo != null)
                        AddToWinningValuesList(vo.actualValues.ToArray());
                }
                AddToWinningValuesList(addedCards);
            }
            else handWasWin = false;
        }

        if (!handWasWin)
            DataLogger.Get().AddToLog("Outcome", "Loss");

        TimedDataLogger.Get().AddToLog(handWasWin ? "FCD Win" : "FCD Loss", "result");

        if (addedCards != null)
            hand.Merge(addedCards);

        int requiredTopUp = 5 - hand.currentHand.Count;
        if (requiredTopUp > 0)
            addedCards = FCD_RiggingSystem.RigForLoss(new ValueOccurenceList(hand.currentHand.ToArray()), requiredTopUp, FCD_Deck.GetInstance(ref deckInstance));
        else addedCards = null;

        if (addedCards != null)
            hand.Merge(addedCards);
    }

    public void FinishTurn()
    {
        if (turn < Globals.balanceCurve.Length)
            turn++;

        FCD_Deck.GetInstance(ref deckInstance).Reset();
    }

    private int ClosestPercentageGainIndex(float balanceVariance, List<int> probableIndices)
    {
        float balanceVariancePercentage = (balanceVariance / Globals.betAmount) * 100;
        float closestPercentageDiff = -1.0f;
        int closestPercentageIndex = -1;

        for (int i = 0; i < probableIndices.Count; i++) {
            float diff = Math.Abs(Globals.returnsPercentages[probableIndices[i]] - balanceVariancePercentage);

            if (closestPercentageIndex == -1 || diff < closestPercentageDiff) {
                closestPercentageDiff = diff;
                closestPercentageIndex = probableIndices[i];
            }
        }

        return closestPercentageIndex;
    }

    private int RandomizeIndexBelowTarget(List<int> probableIndices, int currentIndex)
    {
        int maxIndex = -1;
        for (int i = 0; i < probableIndices.Count; i++) {
            if (probableIndices[i] == currentIndex) {
                maxIndex = i;
                break;
            }
        }

        if (maxIndex != -1) {
            Random rand = new Random();
            return probableIndices[rand.Next(maxIndex + 1)];
        }
        else return currentIndex;
    }

    private void AddToWinningValuesList(int[] arr)
    {
        if (winningValues == null)
            winningValues = new List<int>();

        for (int i = 0; i < arr.Length; i++)
            winningValues.Add(arr[i]);
    }
}