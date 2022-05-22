using System;

namespace ProjektLS22
{
    class RandomAI : PlayerController
    {
        public override int ChooseTrumps(Game g)
        {
            return Utils.rand.Next(-1, 7);
        }

        public override int ChooseTalon(Game g)
        {
            return Utils.rand.Next(player.hand.Count);
        }
        public override int ChoosePlay(Game g)
        {
            return Utils.rand.Next(player.hand.Count);
        }
    }
}