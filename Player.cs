using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class Player
    {
        public int index;
        public PlayerController controller;
        public int defense_wins = 0;
        public int offense_wins = 0;
        public List<Card> hand = new List<Card>();
        public List<Card> discard = new List<Card>();
        public List<Card> marriages = new List<Card>();

        public Player(int index, PlayerController controller)
        {
            this.index = index;
            this.controller = controller;
        }


    }
}