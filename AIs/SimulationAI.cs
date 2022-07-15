using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    public class SimulationAI : SmartAI
    {
        int depth;
        List<Card> talonIfKnown;
        int[] pts = new int[3];
        int offense;
        HashSet<Card> valid = new HashSet<Card>();

        public SimulationAI(int depth)
        {
            this.depth = depth;
        }
        public override void NewRound(int dealer)
        {
            base.NewRound(dealer);
            talonIfKnown = null;
            valid = new HashSet<Card>(Card.ALL);
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, List<Card> talonIfKnown)
        {
            base.FirstTrickStart(trumps, fromPeople, offense, talonIfKnown);
            this.talonIfKnown = talonIfKnown;
            valid.ExceptWith(player.hand);
            this.offense = offense;
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage)
        {
            valid.Remove(c);
            if (marriage)
                pts[p] += 20;
            base.PlaysCard(p, c, trick, trumps, marriage);
        }
        public override void TakesTrick(int p, List<Card> trick)
        {
            pts[p] += trick.Count(c => c.value.ten) * 10;
            base.TakesTrick(p, trick);
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            if (player.hand.Count > depth)
            {
                return base.ChoosePlay(trick, trumps);
            }
            else if (player.hand.Count == 1)
            {
                return 0;
            }
            else
            {
                int b = GetBestPlay(player.index, trumps, player.hand, trick, players, _PPlus(player.index, 3 - trick.Count), offense, pts, talonIfKnown, valid);
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
            _SortCards(ref validCards, trumps.suit, false);
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
                maxCards[offset]--;
                remove--;
                offset = _PPlus(offset, 1);
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
                                    if (predictedHands[h].Contains(c))
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
            List<int> playable = new List<int>();
            for (int i = 0; i < hand.Count; i++)
            {
                if (_ValidPlay(hand, i, trumps, trick))
                {
                    playable.Add(i);
                }
            }
            int[] wins = new int[playable.Count];
            foreach (var permutation in AllPossibleDeals(predictedHands, p, hand, talonIfKnown, startingPlayer, trumps, valid))
            {
                bool[] w = GetWinsForPermutation(p, trumps, permutation, trick, offense, pts, playable);
                for (int i = 0; i < playable.Count; i++)
                {
                    if (w[i])
                        wins[i]++;
                }
            }
            return playable[wins.AsSpan().IndexOf(wins.Max())];
        }
        static bool[] GetWinsForPermutation(int p, Card trumps, List<Card>[] permutation, List<Card> trick, int offense, int[] pts, List<int> playable)
        {
            GameState g = new GameState(permutation, trick, pts, p);
            bool[] wins = new bool[playable.Count];
            for (int i = 0; i < playable.Count; i++)
            {
                GameState n = g.PlayCardAndUpdate(playable[i], trumps.suit);
                bool w = CanWin(n, trumps, offense);
                wins[i] = ((n.player == offense) == (p == offense)) == w;
            }
            return wins;
        }

        static bool CanWin(GameState g, Card trumps, int offense)
        {
            if (g.playerHands[g.player].Count == 0)
            {
                return (g.pts[offense] > g.pts[_PPlus(offense, 1)] + g.pts[_PPlus(offense, 2)]) == (g.player == offense);
            }
            for (int i = 0; i < g.playerHands[g.player].Count; i++)
            {
                if (_ValidPlay(g.playerHands[g.player], i, trumps, g.trick))
                {
                    GameState n = g.PlayCardAndUpdate(i, trumps.suit);
                    bool w = CanWin(n, trumps, offense);
                    if (((n.player == offense) == (g.player == offense)) == w)
                        return true;
                }
            }
            return false;
        }

        struct GameState
        {
            public List<Card>[] playerHands;
            public List<Card> trick;
            public int[] pts;
            public int player;

            public GameState(List<Card>[] permutaion, List<Card> trick, int[] pts, int p)
            {
                playerHands = new List<Card>[3];
                this.pts = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    playerHands[i] = new List<Card>(permutaion[i]);
                    this.pts[i] = pts[i];
                }
                this.trick = new List<Card>(trick);
                player = p;
            }

            public GameState(GameState g)
            {
                playerHands = new List<Card>[3];
                pts = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    playerHands[i] = new List<Card>(g.playerHands[i]);
                    pts[i] = g.pts[i];
                }
                trick = new List<Card>(g.trick);
                player = g.player;
            }

            public GameState PlayCardAndUpdate(int i, Suit trumps)
            {
                GameState n = new GameState(this);
                Card c = playerHands[player][i];
                n.playerHands[player].RemoveAt(i);
                n.trick.Add(c);
                if (c.value.marriage && n.playerHands[player].Any(d => c.SameSuit(d) && d.value.marriage))
                    n.pts[player] += 20;
                n.player = _PPlus(player, 1);
                if (n.trick.Count == 3)
                {
                    int w = _TrickWinner(n.trick, trumps);
                    n.player = _PPlus(player, 1 + w);
                    n.pts[n.player] += n.trick.Count(d => d.value.ten) * 10;
                    n.trick.Clear();
                }
                return n;
            }
        }
    }
}
