using System;
using System.Collections.Generic;
using System.Linq;

namespace FCD_RiggingTools
{
    public enum HandType
    {
        FacePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public static class Globals
    {
        public const string winSequencePath = "initial_seq_w";
        public const string loseSequencePath = "initial_seq_l";
        public const float betAmount = 1.0f;

        public static readonly int[] returnsPercentages = { 100, 200, 300, 400, 600, 900, 2500, 5000, 25000 };
        public static readonly float[] balanceCurve = {
            50,49,48,50,49,48,47,48,47,46,45,46,47,46,45,44,43,42,46,45,44,43,42,41,42,41,40,41,42,41,
            40,39,38,37,39,38,37,36,35,34,35,36,35,34,33,32,31,35,34,33,32,31,32,31,30,29,28,27,28,29,
            28,27,26,25,24,23,25,24,23,22,21,20,21,20,19,18,17,16,18,19,18,17,16,15,14,13,12,14,13,12,
            11,12,13,12,11,10, 9, 8, 7, 6,10, 9, 8, 7, 6, 5, 6, 7, 6, 5, 4, 3, 5, 6, 5, 4, 3, 2, 1, 0
        };
    }

    public class ValueOccurence
    {
        public int simplifiedValue { get; private set; }
        public List<int> actualValues { get; private set; }
        public int occurence { get { return actualValues.Count; } }

        public ValueOccurence(int initialValue)
        {
            simplifiedValue = FCD_Deck.SimplifyValue(initialValue);
            actualValues = new List<int>() { initialValue };
        }
        public ValueOccurence(ValueOccurence other)
        {
            simplifiedValue = other.simplifiedValue;

            actualValues = new List<int>();
            for (int i = 0; i < other.actualValues.Count; i++)
                actualValues.Add(other.actualValues[i]);
        }

        public void AddOccurence(int value)
        {
            if (FCD_Deck.SimplifyValue(value) == simplifiedValue)
                actualValues.Add(value);
        }
    }
    public class ValueOccurenceList
    {
        public List<ValueOccurence> items { get; private set; }
        public int[] variance { get; private set; }

        public ValueOccurenceList(int[] hand, bool royalsOnly = false)
        {
            Initialize(hand, royalsOnly);
        }
        public ValueOccurenceList(ValueOccurence[] voArray)
        {
            List<int> allValues = new List<int>();
            foreach (ValueOccurence vo in voArray) {
                foreach (int i in vo.actualValues)
                    allValues.Add(i);
            }

            Initialize(allValues.ToArray());
        }
        public ValueOccurenceList(ValueOccurenceList other)
        {
            items = new List<ValueOccurence>();
            for (int i = 0; i < other.items.Count; i++)
                items.Add(new ValueOccurence(other.items[i]));
        }

        private void Initialize(int[] hand, bool royalsOnly = false)
        {
            items = new List<ValueOccurence>();

            for (int i = 0; i < hand.Length; i++)
            {
                if (royalsOnly && !FCD_Deck.ValueIsRoyal(hand[i]))
                    continue;

                ValueOccurence match = Find(hand[i]);

                if (match != null)
                    match.AddOccurence(hand[i]);
                else items.Add(new ValueOccurence(hand[i]));
            }

            if (items.Count > 0) {
                items = items.OrderBy(x => x.simplifiedValue).ToList();
                variance = CalculateVariance();
            }
            else variance = new int[0];
        }

        public ValueOccurence Find(int value)
        {
            value = FCD_Deck.SimplifyValue(value);

            for (int i = 0; i < items.Count; i++) {
                if (items[i].simplifiedValue == value)
                    return items[i];
            }

            return null;
        }

        public ValueOccurence FindMostOccuring() { return FindMostOccuringInSimplifiedRange(); }
        public ValueOccurence FindMostOccuringFaceCard() { return FindMostOccuringInSimplifiedRange(FCD_Deck.firstFaceIndex); }
        public ValueOccurence FindMostOccuringInSimplifiedRange(int start = 0, int end = FCD_Deck.valueCount)
        {
            ValueOccurence mostOccuring = null;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].simplifiedValue >= start && items[i].simplifiedValue < end)
                {
                    if (mostOccuring == null || items[i].occurence > mostOccuring.occurence)
                        mostOccuring = items[i];
                }
            }

            return mostOccuring;
        }

        public ValueOccurence[] FindMostOccuringInRange(int range)
        {
            if (variance.Length == 0)
                return items.ToArray();

            int[] bestIndices = null;
            int bestSum = int.MinValue;
            int bestSumIndex = -1;

            for (int i = 0; i < variance.Length; i++) {
                int[] newIndices;
                int newSum = SumLeast(i, out newIndices, range);

                if (newSum > bestSum) {
                    bestSum = newSum;
                    bestSumIndex = i;
                    bestIndices = newIndices;
                }
            }

            if (bestIndices != null)
            {
                ValueOccurence[] outputvo = new ValueOccurence[bestIndices.Length + 1];
                for (int i = 0; i < bestIndices.Length; i++)
                    outputvo[i] = items[bestIndices[i]];
                outputvo[outputvo.Length - 1] = items[bestIndices.Last() + 1];

                return outputvo;
            }
            else return null;
        }
        public ValueOccurence[] FindMostOccuringSuited()
        {
            Dictionary<int, List<ValueOccurence>> foundSuits = new Dictionary<int, List<ValueOccurence>>();

            foreach (ValueOccurence i in items) {
                foreach (int v in i.actualValues) {
                    int suit = v / 13;
                    ValueOccurence newvo = new ValueOccurence(v);

                    if (foundSuits.ContainsKey(suit))
                        foundSuits[suit].Add(newvo);
                    else foundSuits.Add(suit, new List<ValueOccurence>() { newvo });
                }
            }

            int maxSuitIndex = -1;
            foreach (KeyValuePair<int, List<ValueOccurence>> l in foundSuits)
            {
                if (maxSuitIndex == -1 || l.Value.Count > foundSuits[maxSuitIndex].Count)
                    maxSuitIndex = l.Key;
            }

            if (maxSuitIndex != -1)
                return foundSuits[maxSuitIndex].ToArray();
            else return null;
        }

        public bool Contains(int value) { return Find(value) != null; }
        public bool Remove(ValueOccurence vo) { return items.Remove(vo); }

        private int[] CalculateVariance()
        {
            int[] variance = new int[items.Count - 1];
            for (int i = 1; i < items.Count; i++)
                variance[i - 1] = items[i].simplifiedValue - items[i - 1].simplifiedValue;
            return variance;
        }
        private int SumLeast(int startIndex, out int[] associatedIndices, int maxSum = int.MaxValue)
        {
            int sum = variance[startIndex];
            int left = startIndex - 1;
            int right = startIndex + 1;
            List<int> outputIndices = new List<int>();

            if (sum <= maxSum)
                outputIndices.Add(startIndex);
            else {
                associatedIndices = null;
                return -1;
            }

            while ((left >= 0 || right < variance.Length)) {
                int leftValue = (left >= 0) ? variance[left] : int.MaxValue;
                int rightValue = (right < variance.Length) ? variance[right] : int.MaxValue;
                sum += Math.Min(leftValue, rightValue);

                if (sum > maxSum) {
                    sum -= Math.Min(leftValue, rightValue);
                    break;
                }

                if (leftValue < rightValue)
                    outputIndices.Add(left--);
                else outputIndices.Add(right++);
            }

            associatedIndices = outputIndices.ToArray();
            return sum;
        }
    }

    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
