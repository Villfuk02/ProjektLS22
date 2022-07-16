using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Xml;
using static ProjektLS22.Utils;


namespace ProjektLS22
{
    public abstract class GameStateKeepingAI : PlayerController
    {
        protected SpecificPlayerModel[] players = new SpecificPlayerModel[3];
        protected PlayerModel ally;
        protected PlayerModel us;
        protected PlayerModel others;
        protected PlayerModel all;
        protected SpecificPlayerModel me { get => players[player.index]; }
        protected SpecificPlayerModel nextPlayer { get => players[_PPlus(player.index, 1)]; }
        protected SpecificPlayerModel prevPlayer { get => players[_PPlus(player.index, 2)]; }
        protected bool isOffense;
        protected bool isFirstDefense;
        protected bool isSecondDefense;

        public GameStateKeepingAI()
        {
            for (int i = 0; i < 3; i++)
            {
                players[i] = new SpecificPlayerModel();
            }
            all = new JointPlayerModel(players[0], players[1], players[2]);
        }

        public override void NewRound(int dealer)
        {
            isSecondDefense = player.index == dealer;
            isOffense = player.index == _PPlus(dealer, 1);
            isFirstDefense = player.index == _PPlus(dealer, 2);
            all.Reset();
            others = new JointPlayerModel(players[_PPlus(player.index, 1)], players[_PPlus(player.index, 2)]);
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
            us = new JointPlayerModel(me, ally);
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, int offense, List<Card> talonIfKnown)
        {
            if (isOffense)
                others.RemoveRange(talonIfKnown);
            else
                ally.Remove(trumps);
            others.RemoveRange(player.hand);
            me.Set(player.hand);
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage)
        {
            all.Remove(c);
            if (p != player.index)
            {
                Card first = trick[0];
                PlayerModel pm = players[p];
                if (marriage)
                {
                    players[_PPlus(p, 1)].Remove(c.GetPartner());
                    players[_PPlus(p, 2)].Remove(c.GetPartner());
                }
                if (trick.Count == 2)
                {
                    if (first.SameSuit(c))
                    {
                        if (c.LowerThan(first))
                        {
                            pm.RemoveMatching(first.SameSuitButLowerThan);
                        }
                    }
                    else
                    {
                        pm.RemoveMatching(first.SameSuit);
                        if (!trumps.SameSuit(c))
                        {
                            pm.RemoveMatching(trumps.SameSuit);
                        }
                    }
                }
                else if (trick.Count == 3)
                {
                    Card best = _BestFromFirstTwo(trumps, trick);
                    if (first.SameSuit(c))
                    {
                        if (best.SameSuit(first))
                        {
                            if (c.LowerThan(best))
                                pm.RemoveMatching(best.SameSuitButLowerThan);
                        }
                    }
                    else
                    {
                        pm.RemoveMatching(first.SameSuit);
                        if (trumps.SameSuit(c))
                        {
                            if (best.SameSuit(trumps) && c.LowerThan(best))
                            {
                                pm.RemoveMatching(best.SameSuitButLowerThan);
                            }
                        }
                        else
                        {
                            pm.RemoveMatching(trumps.SameSuit);
                        }
                    }
                }
            }
        }
        public override void TakesTrick(int p, List<Card> trick) { }
    }
}
