using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    class RandomAI : PlayerController
    {
        public override int ChooseTrumps()
        {
            return Utils.rand.Next(-1, 7);
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            return Utils.rand.Next(player.hand.Count);
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            return Utils.rand.Next(player.hand.Count);
        }
    }
}