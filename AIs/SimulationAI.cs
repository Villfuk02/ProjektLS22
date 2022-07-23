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
        int maxCombinations;
        Pile? talonIfKnown;
        int[] pts = new int[3];
        int offense;
        SinglePlayerPrediction valid = new SinglePlayerPrediction();

        public SimulationAI(Player p, int depth, int maxCombinations) : base(p)
        {
            this.depth = depth;
            this.maxCombinations = maxCombinations;
        }
        public override void NewRound(int dealer)
        {
            base.NewRound(dealer);
            talonIfKnown = null;
            valid.Reset();
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, Pile? talonIfKnown)
        {
            base.FirstTrickStart(trumps, fromPeople, offense, talonIfKnown);
            this.talonIfKnown = talonIfKnown;
            valid.Remove(Hand);
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
            pts[p] += trick.Count(c => c.Value.GivesPoints) * 10;
            base.TakesTrick(p, trick);
        }
        public override Card ChoosePlay(List<Card> trick, Card trumps)
        {
            if (Hand.Count > depth)
            {
                return base.ChoosePlay(trick, trumps);
            }
            else if (Hand.Count == 1)
            {
                return Hand.Enumerate().First();
            }
            else
            {
                Card preference = base.ChoosePlay(trick, trumps);
                Card b = GetBestPlay(Index, trumps, Hand, trick, players, _PPlus(Index, 3 - trick.Count), offense, pts, talonIfKnown, valid, preference, maxCombinations);
                return b;
            }
        }
        /// <summary>
        /// Get the best card to play.
        /// </summary>
        static Card GetBestPlay(int p, Card trumps, Pile hand, List<Card> trick, SinglePlayerPrediction[] predictedHands, int startingPlayer, int offense, int[] pts, Pile? talonIfKnown, Pile valid, Card preference, int maxCombinations)
        {
            List<Card> playable = new List<Card>();
            foreach (Card c in hand.Enumerate())
            {
                if (_ValidPlay(hand, c, trumps.Suit, trick.ToArray(), trick.Count))
                    playable.Add(c);
            }
            int[] wins = new int[playable.Count];
            int combinations = 0;
            foreach (var combination in CombinationGenerator.AllCombinationsRandomOrder(predictedHands, p, hand, talonIfKnown, startingPlayer, valid))
            {
                combinations++;
                if (combinations > maxCombinations)
                    break;
                bool[] w = GetWinsForCombination(p, trumps, combination, trick, offense, pts, playable);
                for (int i = 0; i < playable.Count; i++)
                {
                    if (w[i])
                        wins[i]++;
                }
            }
            for (int i = 0; i < playable.Count; i++)
            {
                wins[i] *= 2;
                if (playable[i] == preference)
                    wins[i]++;
            }
            return playable[wins.AsSpan().IndexOf(wins.Max())];
        }
        /// <summary>
        /// Which cards win the game given the combination.
        /// </summary>
        static bool[] GetWinsForCombination(int p, Card trumps, Pile[] combination, List<Card> trick, int offense, int[] pts, List<Card> playable)
        {
            GameState g = new GameState(combination, trick, pts, p);
            bool[] wins = new bool[playable.Count];
            for (int i = 0; i < playable.Count; i++)
            {
                GameState n = new GameState(g, playable[i], trumps.Suit);
                bool w = CanWin(n, trumps, offense);
                wins[i] = ((n.player == offense) == (p == offense)) == w;
            }
            return wins;
        }
        /// <summary>
        /// Can the player whose turn it is win from this game state.
        /// </summary>
        static bool CanWin(GameState g, in Card trumps, int offense)
        {
            if (g.playerHands[g.player].IsEmpty)
            {
                return (g.pts[offense] > g.pts[_PPlus(offense, 1)] + g.pts[_PPlus(offense, 2)]) == (g.player == offense);
            }
            foreach (Card c in g.playerHands[g.player].Enumerate())
            {
                if (_ValidPlay(g.playerHands[g.player], c, trumps.Suit, g.trick, g.trickCount))
                {
                    GameState n = new GameState(g, c, trumps.Suit);
                    bool w = CanWin(n, trumps, offense);
                    if (((n.player == offense) == (g.player == offense)) == w)
                        return true;
                }
            }
            return false;
        }

        readonly struct GameState
        {
            public readonly CombinationGenerator.PileTriple playerHands;
            public readonly Card[] trick;
            public readonly int trickCount;
            public readonly int[] pts;
            public readonly int player;

            public GameState(Pile[] combination, List<Card> trick, int[] pts, int p)
            {
                playerHands = new CombinationGenerator.PileTriple(combination[0], combination[1], combination[2]);
                this.pts = new int[3];
                this.trick = new Card[3];
                trickCount = trick.Count;
                for (int i = 0; i < 3; i++)
                {
                    this.pts[i] = pts[i];
                    if (i < trickCount)
                        this.trick[i] = trick[i];
                }
                player = p;
            }
            /// <summary>
            /// Copy the game state and update it by playing the selected card.
            /// </summary>
            public GameState(GameState g, Card cardToPlay, Suit trumps)
            {
                playerHands = new CombinationGenerator.PileTriple(g.playerHands, g.player, (Pile)cardToPlay);
                pts = new int[3];
                trick = new Card[3];
                trickCount = g.trickCount;
                player = g.player;

                trick[trickCount] = cardToPlay;
                trickCount++;
                if (cardToPlay.Value.Marriage && playerHands[player].HasAny(cardToPlay.GetPartner()))
                    pts[player] += 20;
                player = _PPlus(player, 1);
                if (trickCount == 3)
                {
                    int w = _TrickWinner(trick, trumps);
                    player = _PPlus(player, 1 + w);
                    pts[player] += trick.Count(d => d.Value.GivesPoints) * 10;
                    trickCount = 0;
                    if (playerHands[0].Count == 0)
                    {
                        pts[player] += 10;
                    }
                }
            }
        }
    }
}
