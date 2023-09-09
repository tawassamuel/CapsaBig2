using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rule", menuName = "CapsaBig2/Rule Data", order = 0)]
public class CapsaRuleData : ScriptableObject
{
    [System.Serializable]
    public class ComboOutput
    {
        public string name = "Single";
        public int basedNum = 1;
        public int basedValue = 1;

        public ComboOutput()
        {
            name = "Single";
            basedNum = 1;
            basedValue = 1;
        }
    }

    [SerializeField] private List<CardData.Rank> rankOrders = new List<CardData.Rank>();
    [SerializeField] private List<CardData.TypeSymbol> rankSuits = new List<CardData.TypeSymbol>();

    public int GetWeightByCardData(CardData input)
    {
        int wRank = GetRankByRule(input);
        int wSuit = rankSuits.IndexOf(input.GetSuit()) + 1;
        return wRank * 10 + wSuit;
    }

    public int GetRankByRule(CardData input)
    {
        return rankOrders.IndexOf(input.GetRank()) + 1;
    }

    public ComboOutput GetAvailableCombo(int numOfTurn, List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (numOfTurn <= 1)
        {
            bool containDiamon3 = false;
            foreach (CardData card in analyze)
            {
                if (card == null)
                    continue;

                if (card.GetRank() == CardData.Rank.Three && card.GetSuit() == CardData.TypeSymbol.Diamond)
                {
                    containDiamon3 = true;
                    break;
                }
            }

            if (!containDiamon3)
                return null;
        }

        ComboOutput output;
        output = CheckForFullHouse(analyze);
        if (output != null)
            return output;

        output = CheckForStraightFlush(analyze);
        if (output != null)
            return output;

        output = CheckForFourKind(analyze);
        if (output != null)
            return output;

        output = CheckForFlush(analyze);
        if (output != null)
            return output;

        output = CheckForStraight(analyze);
        if (output != null)
            return output;

        output = CheckForThreeKind(analyze);
        if (output != null)
            return output;

        output = CheckForPair(analyze);
        if (output != null)
            return output;

        if (analyze.Count == 1)
        {
            output = new ComboOutput();
            output.basedNum = 1;
            output.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            output.name = "Single";

            return output;
        }    

        return null;
    }

    public ComboOutput CheckForPair(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 2)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 2);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 2;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Pair";
            return outputCombo;
        }
    }

    public ComboOutput CheckForThreeKind(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 3)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 3);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 3;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Three of a kind";
            return outputCombo;
        }
    }

    public ComboOutput CheckForStraight(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5)
            return null;

        analyze = analyze.GroupBy(data => GetRankByRule(data)).Select(y => y.First()).ToList();
        analyze = analyze.OrderBy(data => GetRankByRule(data)).ToList();

        bool fetchAnalyzer = analyze.Zip(analyze.Skip(4), (x, y) => (GetRankByRule(x) + 4) == GetRankByRule(y)).Any(x => x);
        if (fetchAnalyzer)
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Straight";
            return outputCombo;
        }

        return null;
    }

    public ComboOutput CheckForFourKind(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 5)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 4);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Four of a kind & one card";
            return outputCombo;
        }
    }

    public ComboOutput CheckForFlush(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => data.GetSuit()).Count() == 1;
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Flush";
            return outputCombo;
        }
    }

    public ComboOutput CheckForStraightFlush(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5)
            return null;

        bool fetchFlush = (CheckForFlush(analyze) != null);

        if (!fetchFlush)
            return null;

        bool fetchAnalyzer = (CheckForStraight(analyze) != null);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Straight Flush";
            return outputCombo;
        }
    }

    public ComboOutput CheckForFullHouse(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Count() == 2 && analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 3);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.name = "Full House";
            return outputCombo;
        }
    }
}
