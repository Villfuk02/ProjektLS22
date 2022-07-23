using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Xml;
using static ProjektLS22.Utils;


namespace ProjektLS22
{
    /// <summary>
    /// Just keeps track of what cars can which player have.
    /// </summary>
    public abstract class GameStateKeepingAI : PlayerController
    {
        protected SinglePlayerPrediction[] players = new SinglePlayerPrediction[3];
        protected PlayerPrediction ally;
        protected PlayerPrediction us;
        protected PlayerPrediction others;
        protected PlayerPrediction all;
        protected SinglePlayerPrediction me { get => players[Index]; }
        protected SinglePlayerPrediction nextPlayer { get => players[_PPlus(Index, 1)]; }
        protected SinglePlayerPrediction prevPlayer { get => players[_PPlus(Index, 2)]; }
        protected bool isOffense;
        protected bool isFirstDefense;
        protected bool isSecondDefense;

        public GameStateKeepingAI(Player p) : base(p, false)
        {
            for (int i = 0; i < 3; i++)
            {
                players[i] = new SinglePlayerPrediction();
            }
            all = new JointPlayerPrediction(players[0], players[1], players[2]);
        }

        public override void NewRound(int dealer)
        {
            isSecondDefense = Index == dealer;
            isOffense = Index == _PPlus(dealer, 1);
            isFirstDefense = Index == _PPlus(dealer, 2);
            all.Reset();
            others = new JointPlayerPrediction(players[_PPlus(Index, 1)], players[_PPlus(Index, 2)]);
            if (isOffense)
            {
                ally = null;
            }
            else if (isFirstDefense)
            {
                ally = nextPlayer;
            }
            else
            {
                ally = prevPlayer;
            }
            us = new JointPlayerPrediction(me, ally);
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, Pile? talonIfKnown)
        {
            if (isOffense)
                others.Remove(talonIfKnown.Value);
            else
                ally.Remove(trumps);
            others.Remove(Hand);
            me.Set(Hand);
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage)
        {
            all.Remove(c);
            if (p != Index)
            {
                Card first = trick[0];
                PlayerPrediction pp = players[p];
                if (marriage)
                {
                    players[_PPlus(p, 1)].Remove(c.GetPartner());
                    players[_PPlus(p, 2)].Remove(c.GetPartner());
                }
                if (trick.Count == 2)
                {
                    if (first.SameSuit(c))
                    {
                        if (c.Value < first.Value)
                        {
                            pp.Remove(first.SameSuitButLower);
                        }
                    }
                    else
                    {
                        pp.Remove(first.Suit);
                        if (!trumps.SameSuit(c))
                        {
                            pp.Remove(trumps.Suit);
                        }
                    }
                }
                else if (trick.Count == 3)
                {
                    Card best = _BestFromFirstTwo(trumps.Suit, trick.ToArray(), trick.Count);
                    if (first.SameSuit(c))
                    {
                        if (best.SameSuit(first))
                        {
                            if (c.Value < best.Value)
                                pp.Remove(best.SameSuitButLower);
                        }
                    }
                    else
                    {
                        pp.Remove(first.Suit);
                        if (trumps.SameSuit(c))
                        {
                            if (best.SameSuit(trumps) && c.Value < best.Value)
                            {
                                pp.Remove(best.SameSuitButLower);
                            }
                        }
                        else
                        {
                            pp.Remove(trumps.Suit);
                        }
                    }
                }
            }
        }
        public override void TakesTrick(int p, List<Card> trick) { }
    }
}
