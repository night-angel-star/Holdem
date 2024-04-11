using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandStrengthCalculator
{
    public enum Rank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public static int CalculateHandStrength(List<string> allCards)
    {

        if (HasRoyalFlush(allCards))
            return 10;

        if (HasStraightFlush(allCards, out Rank straightFlushRank))
            return 9;

        if (HasFourOfAKind(allCards))
            return 8;

        if (HasFullHouse(allCards))
            return 7;

        if (HasFlush(allCards))
            return 6;

        if (HasStraight(allCards))
            return 5;

        if (HasThreeOfAKind(allCards))
            return 4;

        if (HasTwoPair(allCards))
            return 3;

        if (HasOnePair(allCards))
            return 2;

        return 1;
    }

    private static bool HasRoyalFlush(List<string> cards)
    {
        List<string> suits = cards.Select(card => card.Substring(1)).Distinct().ToList();

        if (suits.Count == 1)
        {
            string suit = suits.First();
            List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

            if (ranks.Contains("A") && ranks.Contains("K") && ranks.Contains("Q") && ranks.Contains("J") && ranks.Contains("T"))
                return true;
        }

        return false;
    }

    private static bool HasStraightFlush(List<string> cards, out Rank rank)
    {
        List<string> suits = cards.Select(card => card.Substring(1)).Distinct().ToList();

        if (suits.Count == 1)
        {
            List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

            if (ranks.Count >= 5)
            {
                List<int> sortedRanks = ranks.Select(rank => GetRankValue(rank)).OrderByDescending(rank => rank).ToList();

                for (int i = 0; i <= sortedRanks.Count - 5; i++)
                {
                    bool isStraight = true;
                    for (int j = 0; j < 5; j++)
                    {
                        if (sortedRanks[i] - j != sortedRanks[i + j])
                        {
                            isStraight = false;
                            break;
                        }
                    }

                    if (isStraight)
                    {
                        rank = Rank.StraightFlush;
                        return true;
                    }
                }
            }
        }

        rank = Rank.HighCard;
        return false;
    }

    private static bool HasFourOfAKind(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

        foreach (string rank in ranks)
        {
            int count = cards.Count(card => card.Substring(0, 1) == rank);
            if (count >= 4)
                return true;
        }

        return false;
    }

    private static bool HasFullHouse(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

        if (ranks.Count >= 2)
        {
            foreach (string rank in ranks)
            {
                int count = cards.Count(card => card.Substring(0, 1) == rank);
                if (count >= 3)
                {
                    List<string> otherRanks = ranks.Where(r => r != rank).ToList();
                    foreach (string otherRank in otherRanks)
                    {
                        int otherCount = cards.Count(card => card.Substring(0, 1) == otherRank);
                        if (otherCount >= 2)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool HasFlush(List<string> cards)
    {
        if (cards.Count >= 5)
        {
            List<string> suits = cards.Select(card => card.Substring(1)).Distinct().ToList();

            if (suits.Count == 1)
                return true;
        }
        

        return false;
    }

    private static bool HasStraight(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

        if (ranks.Count >= 5)
        {
            List<int> sortedRanks = ranks.Select(rank => GetRankValue(rank)).OrderByDescending(rank => rank).ToList();

            for (int i = 0; i <= sortedRanks.Count - 5; i++)
            {
                bool isStraight = true;
                for (int j = 0; j < 5; j++)
                {
                    if (sortedRanks[i] - j != sortedRanks[i + j])
                    {
                        isStraight = false;
                        break;
                    }
                }

                if (isStraight)
                    return true;
            }
        }

        return false;
    }

    private static bool HasThreeOfAKind(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

        foreach (string rank in ranks)
        {
            int count = cards.Count(card => card.Substring(0, 1) == rank);
            if (count >= 3)
                return true;
        }

        return false;
    }

    private static bool HasTwoPair(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();
        int pairCount = 0;

        foreach (string rank in ranks)
        {
            int count = cards.Count(card => card.Substring(0, 1) == rank);
            if (count >= 2)
                pairCount++;
        }

        if (pairCount >= 2)
            return true;

        return false;
    }

    private static bool HasOnePair(List<string> cards)
    {
        List<string> ranks = cards.Select(card => card.Substring(0, 1)).Distinct().ToList();

        foreach (string rank in ranks)
        {
            int count = cards.Count(card => card.Substring(0, 1) == rank);
            if (count >= 2)
                return true;
        }

        return false;
    }

    private static int GetRankValue(string rank)
    {
        switch (rank)
        {
            case "A":
                return 14;
            case "K":
                return 13;
            case "Q":
                return 12;
            case "J":
                return 11;
            case "T":
                return 10;
            default:
                return int.Parse(rank);
        }
    }
}
