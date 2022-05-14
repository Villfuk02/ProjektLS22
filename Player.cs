using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class Player
    {
        public int index;
        public PlayerController controller;
        public int cash = 50;
        public List<Card> hand = new List<Card>();
        public List<Card> discard = new List<Card>();

        public Player(int index, PlayerController controller)
        {
            this.index = index;
            this.controller = controller;
        }


    }
}