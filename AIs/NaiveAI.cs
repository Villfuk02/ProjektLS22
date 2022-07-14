using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjektLS22
{
    class NaiveAI : PlayerController
    {
        int toTry = 0;
        bool isOffense;
        Dictionary<Suit, SuitInfo> suits = new Dictionary<Suit, SuitInfo>();

        public override void NewRound(int dealer)
        {
            isOffense = player.index == (dealer + 1) % 3;
            suits.Clear();
            foreach (Suit s in Suit.ALL)
            {
                SuitInfo i = new SuitInfo();
                i.suit = s;
                i.enemyCountMax = 8;
                suits.Add(s, i);
            }
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, List<Card> talonIfKnown)
        {
            foreach (Card c in player.hand)
            {
                SuitInfo si = suits[c.suit];
                si.count++;
                si.enemyCountMax--;
                if (c.value == Value.DESET)
                    si.haveTen = true;
                else if (c.value == Value.ESO)
                    si.haveAce = true;
                suits[c.suit] = si;
            }
            if (talonIfKnown == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    suits[Suit.ALL[i]].enemyCountMin = suits[Suit.ALL[i]].enemyCountMax - 2;
                }
            }
            else
            {
                foreach (Card c in talonIfKnown)
                {
                    suits[c.suit].enemyCountMax--;
                }
            }
            if (!isOffense)
            {
                suits[trumps.suit].enemyCountMin++;
            }
            suits[trumps.suit].trumps = true;
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage)
        {
            if (p == player.index)
            {
                suits[c.suit].count--;
                if (c.value == Value.DESET)
                    suits[c.suit].haveTen = false;
                else if (c.value == Value.ESO)
                    suits[c.suit].haveAce = false;
            }
            else
            {
                suits[c.suit].enemyCountMax--;
                suits[c.suit].enemyCountMin--;
            }
            if (c.value == Value.DESET)
                suits[c.suit].playedTen = false;
            else if (c.value == Value.ESO)
                suits[c.suit].playedAce = false;
        }
        public override void TakesTrick(int p, List<Card> trick)
        {
            toTry = 0;
        }
        public override int ChooseTrumps()
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>();
            for (int i = 0; i < 7; i++)
            {
                Card c = player.hand[i];
                if (strengths.ContainsKey(c.suit))
                    strengths[c.suit] += c.value.gameStrength;
                else
                    strengths[c.suit] = c.value.gameStrength;
            }
            KeyValuePair<Suit, int> max = new KeyValuePair<Suit, int>(null, 0);
            int average = 0;
            foreach (KeyValuePair<Suit, int> p in strengths)
            {
                if (p.Value > max.Value)
                    max = p;
                average += p.Value;
            }
            average /= 4;
            if (max.Value < 37 || max.Value - average < 10)
            {
                return -1;
            }
            for (int i = 0; i < 7; i++)
            {
                if (player.hand[i].suit == max.Key)
                    return i;
            }
            //SHOULD NEVER HAPPEN
            Renderer.PRINT.P($" {Utils.TranslatePlayer(player.index)} trump choice error ");
            Renderer.PrintHand(player.hand, true, false);
            return -2;
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>();
            HashSet<Suit> married = new HashSet<Suit>();
            for (int i = 0; i < player.hand.Count; i++)
            {
                Card c = player.hand[i];
                if (i != 0 && c.value.marriage && player.hand[i - 1].value.marriage && c.suit == player.hand[i - 1].suit)
                {
                    married.Add(c.suit);
                }
                if (strengths.ContainsKey(c.suit))
                    strengths[c.suit] += c.value.gameStrength;
                else
                    strengths[c.suit] = c.value.gameStrength;
            }
            strengths[trumps.suit] = 999;
            var sorted = from entry in strengths orderby entry.Value ascending select entry.Key;
            foreach (Suit s in sorted)
            {
                for (int i = 0; i < player.hand.Count; i++)
                {
                    Card c = player.hand[i];
                    if (c.suit == s)
                    {
                        if (Utils.ValidTalon(player.hand, i, trumps, null))
                        {
                            if (c.value.marriage && married.Contains(s))
                            {
                                continue;
                            }
                            return i;
                        }
                    }
                }
            }
            return Utils.rand.Next(player.hand.Count);
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            if (trick.Count == 0)
            {
                if (isOffense)
                {
                    Suit chosen = null;
                    if (suits[trumps.suit].enemyCountMax <= 0)
                    {
                        var sorted = from entry in suits orderby entry.Value.enemyCountMax <= 0 descending select entry.Key;
                        foreach (Suit s in sorted)
                        {
                            if (!suits[s].trumps && ((suits[s].haveAce && (suits[s].haveTen || suits[s].playedTen) || suits[s].haveTen && suits[s].playedAce)))
                            {
                                chosen = s;
                                break;
                            }
                        }
                        for (int i = player.hand.Count - 1; i >= 0; i--)
                        {
                            if (player.hand[i].suit == chosen)
                                return i;
                        }
                        return toTry++;
                    }
                    else
                    {
                        var sorted = from entry in suits orderby entry.Value.enemyCountMax - entry.Value.count ascending select entry.Key;
                        foreach (Suit s in sorted)
                        {
                            if (suits[s].enemyCountMax > suits[s].count)
                                break;
                            if (suits[s].count > 0 && !suits[s].trumps && !player.hand.Find((Card c) => c.suit == s).value.ten)
                            {
                                chosen = s;
                                break;
                            }
                        }
                        if (suits[trumps.suit].enemyCountMax < suits[trumps.suit].count && suits[trumps.suit].count > 0)
                            chosen = trumps.suit;
                        for (int i = 0; i < player.hand.Count; i++)
                        {
                            if (player.hand[i].suit == chosen)
                                return i;
                        }
                        return toTry++;
                    }
                }
                else
                {
                    return Utils.rand.Next(player.hand.Count);
                }

            }
            else
            {
                return toTry++;
            }
        }
    }

    public class SuitInfo
    {
        public Suit suit;
        public int count;
        public int enemyCountMin;
        public int enemyCountMax;
        public bool haveAce;
        public bool playedAce;
        public bool haveTen;
        public bool playedTen;
        public bool trumps;
    }
}

