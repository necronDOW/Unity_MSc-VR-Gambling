using System;
using System.IO;
using System.Collections.Generic;
using FCD_RiggingTools;

class FCD_DealerAssistant
{
    public class Sequence
    {
        public bool valid { get { return allData != null; } }
        private int returnsPercentage;
        private int countPerSet = -1;
        private int[] allData;

        public Sequence(string[] lines, int startIndex, int endIndex)
        {
            int indexer = 0;
            for (int i = startIndex; i < endIndex; i++) {
                string[] tmpSplit = lines[i].Split(':');
                string[] values = tmpSplit[1].Split(',');

                int returnsPercentageIndex = -1;
                if (i == startIndex) {
                    if (!int.TryParse(tmpSplit[0], out returnsPercentageIndex))
                        return;
                    else if (returnsPercentageIndex >= 0 && returnsPercentageIndex < Globals.returnsPercentages.Length)
                        returnsPercentage = Globals.returnsPercentages[returnsPercentageIndex];

                    countPerSet = values.Length;
                    allData = new int[(endIndex - startIndex) * countPerSet];
                }

                for (int j = 0; j < values.Length; j++) {
                    if (!int.TryParse(values[j], out allData[indexer++])) {
                        allData = null;
                        return;
                    }
                }
            }
        }

        public int[] GetRandomSet()
        {
            Random rnd = new Random();
            int index = rnd.Next(0, allData.Length / countPerSet) * countPerSet;
            int[] set = new int[countPerSet];

            for (int i = 0; i < countPerSet; i++)
                set[i] = allData[index + i];

            return set;
        }
    }

    private Sequence[] winSequence, loseSequence;
    private int simulatedDeckInstance = -1;
    private FCD_Deck simulatedDeck;

    private static FCD_DealerAssistant _instance;
    public static FCD_DealerAssistant Instance
    {
        get
        {
            if (_instance == null)
                _instance = new FCD_DealerAssistant();
            return _instance;
        }
    }

    public FCD_DealerAssistant()
    {
        simulatedDeck = FCD_Deck.GetInstance(ref simulatedDeckInstance);
    }

    public bool InitializeWinSequence(string path)
    {
        if (winSequence == null)
            return (winSequence = GenerateSequences(path)) != null;
        else return false;
    }
    public bool InitializeLoseSequence(string path)
    {
        if (loseSequence == null)
            return (loseSequence = GenerateSequences(path)) != null;
        else return false;
    }

    public int[] GetCardSet(float balanceOffset)
    {
        int[] set;

        //if (balanceOffset == 0.0f)
        //    set = RandomHand();
        //else
        //{
        //    if (balanceOffset > 0.0f)
        //        set = loseSequence[0].GetRandomSet();
        //    else set = winSequence[0].GetRandomSet();

        //    ScrambleSet(ref set);
        //    RandomizeSuits(ref set);
        //}

        //set = new int[5] { 0, 0, 1, 10, 10 };
        set = new int[5] { 8, 9, 10, 12, 12 };

        simulatedDeck.Reset();
        return set;
    }

    private int[] RandomHand()
    {
        int[] hand = new int[5];

        for (int i = 0; i < hand.Length; i++)
            hand[i] = simulatedDeck.DrawRandomCard();

        return hand;
    }

    private void ScrambleSet(ref int[] set)
    {
        List<int> newSet = new List<int>();
        Random rnd = new Random();

        while (newSet.Count < set.Length)
        {
            int index = rnd.Next(0, set.Length);
            if (set[index] != -1)
            {
                newSet.Add(set[index]);
                set[index] = -1;
            }
        }

        set = newSet.ToArray();
    }

    private void RandomizeSuits(ref int[] set)
    {
        int randomized = 0;
        Random rnd = new Random();

        while (randomized < set.Length)
        {
            int randomSuit = rnd.Next(0, FCD_Deck.suitCount);
            int cardIndex = FCD_Deck.valueCount * randomSuit + set[randomized];

            if (simulatedDeck.DrawCard(cardIndex) != -1)
                set[randomized++] = cardIndex;
        }
    }

    private Sequence[] GenerateSequences(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        string[] allLines = File.ReadAllLines(filePath);
        List<Sequence> foundSequences = new List<Sequence>();

        for (int start = 0; start < allLines.Length; start++) {
            string targetHandTypeId = allLines[start].Split(':')[0];

            for (int end = start; end < allLines.Length; end++) {
                if (end == allLines.Length - 1) {
                    foundSequences.Add(new Sequence(allLines, start, ++end));
                    start = end;
                    break;
                }

                if (targetHandTypeId != allLines[end].Split(':')[0]) {
                    foundSequences.Add(new Sequence(allLines, start, end));
                    start = end - 1;
                    break;
                }
            }
        }
        
        return foundSequences.ToArray();
    }

    public static bool GetRandomizedWin(float walletBalance, float curveBalanceTarget, float betAmount)
    {
        return GetRandomizedWin((walletBalance - curveBalanceTarget), betAmount);
    }
    public static bool GetRandomizedWin(float balanceVariance, float betAmount)
    {
        Random rand = new Random();

        int betsOffset = (int)(balanceVariance / betAmount);

        int pow = (int)Math.Pow(2, Math.Abs(betsOffset));
        if (pow == int.MinValue || pow > int.MaxValue - 2)
            pow = int.MaxValue - 2;

        int minRange = -2 - (betsOffset > 0 ? pow : 0);
        int maxRange = 2 + (betsOffset < 0 ? pow : 0);

        return (rand.Next(minRange, maxRange) >= 0);
    }

    public static bool GetWin(float balanceVariance)
    {
        if (balanceVariance < 0)
            return true;
        else return false;
    }
}
