using System;
using System.Collections.Generic;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    class RandomAI : PlayerController
    {
        public override int ChooseTrumps()
        {
            return _rand.Next(-1, 7);
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            return _rand.Next(player.hand.Count);
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            return _rand.Next(player.hand.Count);
        }
    }
}