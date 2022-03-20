using System;

namespace ProjektLS22
{
    public class Player
    {
        World w;
        public ConsoleColor color;
        public int energy = 50;

        public Player(World w, ConsoleColor color)
        {
            this.w = w;
            this.color = color;
        }
    }
}