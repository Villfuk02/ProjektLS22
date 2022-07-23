using System;
using System.Collections.Generic;
using System.Linq;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    class RandomAI : PlayerController
    {
        public RandomAI(Player p) : base(p, false) { }
        public override Card? ChooseTrumps()
        {
            return Hand.Enumerate().ElementAt(_rand.Next(Hand.Count));
        }
        public override Card ChooseTalon(Card trumps, Pile talon)
        {
            return Hand.Enumerate().ElementAt(_rand.Next(Hand.Count));
        }
        public override Card ChoosePlay(List<Card> trick, Card trumps)
        {
            return Hand.Enumerate().ElementAt(_rand.Next(Hand.Count));
        }
    }
}