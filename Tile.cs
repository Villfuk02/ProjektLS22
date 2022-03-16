using System;

namespace ProjektLS22
{
    public class Tile
    {
        World w;
        Pos p;
        bool blocker;
        public ConsoleColor color;
        //building
        public Tile(World w, Pos p, bool blocker)
        {
            this.w = w;
            this.p = p;
            this.blocker = blocker;
            color = blocker ? ConsoleColor.Gray : ConsoleColor.Black;
        }
    }
}