using System;

namespace ProjektLS22
{
    public class Player
    {
        ConsoleColor color;
        PlayerController controller;
        public int energy = 50;

        public Player(ConsoleColor color, PlayerController controller)
        {
            this.color = color;
            this.controller = controller;
        }
    }
}