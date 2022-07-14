using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace ProjektLS22
{
    public class SimulationAI : PlayerController
    {
        SpecificPlayerModel[] playerModels = new SpecificPlayerModel[3];
        SpecificPlayerModel me;
        SpecificPlayerModel P(int n)
        {
            return playerModels[n % 3];
        }

        bool isOffense;
        bool isFirstDefense;
        List<Card> talonIfKnown;
        int[] pts = new int[3];
        int offense;

        string[] playerLog = new string[3];
        string[] trickLog = new string[10];
        int tricknum = 0;
        HashSet<Card> valid = new HashSet<Card>();

        public SimulationAI()
        {
            for (int i = 0; i < 3; i++)
            {
                playerModels[i] = new SpecificPlayerModel();
            }
        }

        public override void NewRound(int dealer)
        {
            isOffense = player.index == (dealer + 1) % 3;
            isFirstDefense = player.index == (dealer + 2) % 3;
            for (int i = 0; i < 3; i++)
            {
                playerModels[i].Reset();
                pts[i] = 0;
                playerLog[i] = "";
            }
            trickLog = new string[10];
            tricknum = -1;
            me = playerModels[player.index];
            talonIfKnown = null;
            valid = new HashSet<Card>(Card.ALL);
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, List<Card> talonIfKnown)
        {
            this.talonIfKnown = talonIfKnown;
            if (isOffense)
            {
                for (int i = 1; i <= 2; i++)
                {
                    P(player.index + i).RemoveRange(talonIfKnown);
                }
            }
            else
            {
                for (int i = 1; i <= 2; i++)
                {
                    P(offense + i).Remove(trumps);
                }
            }
            for (int i = 1; i <= 2; i++)
            {
                P(player.index + i).RemoveRange(player.hand);
            }
            me.RemoveMatching((Card c) => !player.hand.Contains(c));
            valid.ExceptWith(player.hand);
            this.offense = offense;
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card t, bool marrige)
        {
            valid.Remove(c);
            Suit trumps = t.suit;
            playerLog[p] += Utils.FormatCards(new List<Card> { c }, trumps);
            if (trick.Count == 1)
            {
                tricknum++;
            }
            trickLog[tricknum] += Utils.FormatCards(new List<Card> { c }, trumps);
            if (marrige)
                pts[p] += 20;
            for (int i = 0; i < 3; i++)
            {
                P(p + i).Remove(c);
            }
            if (p != player.index)
            {
                if (marrige)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        P(p + i).Remove(c.value == Value.KRÁL ? new Card(c.suit, Value.SVRŠEK) : new Card(c.suit, Value.KRÁL));
                    }
                }
                if (trick.Count == 2)
                {
                    if (c.suit == trick[0].suit)
                    {
                        if (c.value.gameStrength < trick[0].value.gameStrength)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trick[0].suit && d.value.gameStrength > trick[0].value.gameStrength);
                        }
                    }
                    else
                    {
                        P(p).RemoveMatching((Card d) => d.suit == trick[0].suit);
                        if (c.suit != trumps)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trumps);
                        }
                    }
                }
                else if (trick.Count == 3)
                {
                    Card best = trick[0];
                    if (trick.Count > 1)
                    {
                        if (trick[1].suit == trick[0].suit)
                        {
                            if (trick[1].value.gameStrength > trick[0].value.gameStrength)
                                best = trick[1];
                        }
                        else if (trick[1].suit == trumps)
                        {
                            best = trick[1];
                        }
                    }
                    if (c.suit == trick[0].suit)
                    {
                        if (best.suit == trick[0].suit)
                        {
                            if (c.value.gameStrength < best.value.gameStrength)
                            {
                                P(p).RemoveMatching((Card d) => d.suit == c.suit && d.value.gameStrength > best.value.gameStrength);
                            }
                        }
                    }
                    else
                    {
                        P(p).RemoveMatching((Card d) => d.suit == trick[0].suit);
                        if (c.suit != trumps)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trumps);
                        }
                        else
                        {
                            if (best.suit == trumps && c.value.gameStrength < best.value.gameStrength)
                            {
                                P(p).RemoveMatching((Card d) => d.suit == trumps && d.value.gameStrength > best.value.gameStrength);
                            }
                        }
                    }
                }
            }
        }
        public override void TakesTrick(int p, List<Card> trick)
        {
            foreach (Card c in trick)
            {
                if (c.value.ten)
                    pts[p] += 10;
            }
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
            if (player.hand.Count > 3)
            {
                return Utils.rand.Next(player.hand.Count);
            }
            else if (player.hand.Count == 1)
            {
                return 0;
            }
            else
            {
                int b = GetBestPlay(player.index, trumps, player.hand, trick, playerModels, (player.index + 3 - trick.Count) % 3, offense, pts, talonIfKnown, valid);
                return b;
            }
        }

        static IEnumerable<List<Card>[]> AllPossibleDeals(SpecificPlayerModel[] predictedHands, int fixedPlayer, List<Card> hand, List<Card> talonIfKnown, int startingPlayer, Card trumps, IEnumerable<Card> valid)
        {
            List<Card>[] permutation = new List<Card>[4];
            for (int i = 0; i < 4; i++)
            {
                permutation[i] = new List<Card>();
            }
            permutation[fixedPlayer] = hand;
            if (talonIfKnown != null)
                permutation[3] = talonIfKnown;
            List<Card> validCards = new List<Card>(valid);
            Utils.SortCards(ref validCards, trumps.suit, false);
            validCards.RemoveAll(d => permutation[3].Contains(d));
            int[] maxCards = new int[4];
            int remove = hand.Count * 2 - validCards.Count + 5 - permutation[3].Count;
            for (int i = 0; i < 3; i++)
            {
                maxCards[i] = hand.Count + 1;
            }
            maxCards[3] = 2;
            int offset = startingPlayer;
            while (remove > 0)
            {
                maxCards[offset % 3]--;
                remove--;
                offset++;
            }
            Stack<int> additions = new Stack<int>();
            int pos = 0;
            while (pos >= 0)
            {
                if (pos >= validCards.Count)
                {
                    yield return permutation;
                    pos--;
                    if (pos >= 0)
                    {
                        int l = additions.Peek();
                        permutation[l].Remove(validCards[pos]);
                    }
                }
                else
                {
                    if (additions.Count == pos)
                    {
                        additions.Push(-1);
                    }
                    else
                    {
                        int h = additions.Pop() + 1;
                        Card c = validCards[pos];
                        if (h == 4)
                        {
                            pos--;
                            if (pos >= 0)
                            {
                                int l = additions.Peek();
                                permutation[l].Remove(validCards[pos]);
                            }
                        }
                        else
                        {
                            additions.Push(h);
                            if (maxCards[h] > permutation[h].Count)
                            {
                                if (h < 3)
                                {
                                    if (predictedHands[h].MayHave().Contains(c))
                                    {
                                        permutation[h].Add(c);
                                        pos++;
                                    }
                                }
                                else
                                {
                                    if (!c.value.ten && c != trumps)
                                    {
                                        permutation[h].Add(c);
                                        pos++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static int GetBestPlay(int p, Card trumps, List<Card> hand, List<Card> trick, SpecificPlayerModel[] predictedHands, int startingPlayer, int offense, int[] pts, List<Card> talonIfKnown, IEnumerable<Card> valid)
        {
            int numP = 0;
            int[] counts = new int[4];
            // Renderer.PRINT.NL().P(pts[0], 3).S(1).P(pts[1], 3).S(1).P(pts[2], 3);
            List<int> playable = new List<int>();
            for (int i = 0; i < hand.Count; i++)
            {
                if (Utils.ValidPlay(hand, i, trumps, trick))
                {
                    playable.Add(i);
                }
            }
            int[] wins = new int[playable.Count];
            foreach (var permutation in AllPossibleDeals(predictedHands, p, hand, talonIfKnown, startingPlayer, trumps, valid))
            {
                numP++;
                // Renderer.PRINT.NL();
                for (int i = 0; i < 4; i++)
                {
                    // Renderer.PrintHand(permutation[i], true, false);
                    // Renderer.PRINT.S(3);
                    if (numP > 1 && permutation[i].Count != counts[i])
                        ;
                    counts[i] = permutation[i].Count;
                }
                // Renderer.PRINT.NL();
                bool[] w = GetWinsForPermutation(p, trumps, permutation, trick, offense, pts, playable);
                for (int i = 0; i < playable.Count; i++)
                {
                    wins[i] += w[i] ? 1 : 0;
                    // Renderer.PRINT.P(w[i] ? 'W' : 'L').S(1);
                }
            }
            // Renderer.PRINT.NL();
            for (int i = 0; i < playable.Count; i++)
            {
                // Renderer.PRINT.P(wins[i], 6).S(1);
            }
            return playable[wins.AsSpan().IndexOf(wins.Max())];
        }
        static bool[] GetWinsForPermutation(int p, Card trumps, List<Card>[] permutation, List<Card> trick, int offense, int[] pts, List<int> playable)
        {
            bool[] wins = new bool[playable.Count];
            for (int i = 0; i < playable.Count; i++)
            {
                (List<Card>[] newPermutation, List<Card> newTrick, int[] newPts, int newP) = PlayCardAndUpdate(p, playable[i], permutation, trick, pts, trumps.suit);
                bool w = CanWin(newP, trumps, newPermutation, newTrick, offense, newPts);
                wins[i] = ((newP == offense) == (p == offense)) == w;
            }
            return wins;
        }

        static bool CanWin(int p, Card trumps, List<Card>[] permutation, List<Card> trick, int offense, int[] pts)
        {
            if (permutation[p].Count == 0)
            {
                return (pts[offense] > pts[(offense + 1) % 3] + pts[(offense + 2) % 3]) == (p == offense);
            }
            for (int i = 0; i < permutation[p].Count; i++)
            {
                if (Utils.ValidPlay(permutation[p], i, trumps, trick))
                {
                    (List<Card>[] newPermutation, List<Card> newTrick, int[] newPts, int newP) = PlayCardAndUpdate(p, i, permutation, trick, pts, trumps.suit);
                    bool w = CanWin(newP, trumps, newPermutation, newTrick, offense, newPts);
                    if (((newP == offense) == (p == offense)) == w)
                        return true;
                }
            }
            return false;
        }

        static (List<Card>[], List<Card>, int[], int) PlayCardAndUpdate(int p, int i, List<Card>[] permutation, List<Card> trick, int[] pts, Suit trumps)
        {
            List<Card>[] newPermutation = new List<Card>[4];
            for (int j = 0; j < 4; j++)
            {
                newPermutation[j] = new List<Card>(permutation[j].Select(c => new Card(c.suit, c.value)));
            }
            Card c = newPermutation[p][i];
            newPermutation[p].RemoveAt(i);
            List<Card> newTrick = new List<Card>(trick);
            newTrick.Add(c);
            int[] newPts = new int[3];
            for (int j = 0; j < 3; j++)
            {
                newPts[j] = pts[j];
            }
            if (c.value.marriage && newPermutation[p].Any(d => d.suit == c.suit && d.value.marriage))
                newPts[p] += 20;
            int newP = (p + 1) % 3;
            if (newTrick.Count == 3)
            {
                int w = Utils.TrickWinner(newTrick, trumps);
                newP = (p + 1 + w) % 3;
                newPts[newP] += newTrick.Count(d => d.value.ten) * 10;
                newTrick.Clear();
            }
            return (newPermutation, newTrick, newPts, newP);
        }
    }
}
