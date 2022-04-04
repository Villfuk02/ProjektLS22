using System;

namespace ProjektLS22
{
    public class Tile
    {
        World w;
        public Pos p;
        bool blocker;
        public ConsoleColor color;
        public Building b;
        public Tile(World w, Pos p, bool blocker)
        {
            this.w = w;
            this.p = p;
            this.blocker = blocker;
            color = blocker ? ConsoleColor.Gray : ConsoleColor.Black;
        }

        public void Place(Player p, World w, Building b)
        {
            this.b = b;
            b.OnPlace(p, this, w);
        }

        public bool IsEmpty()
        {
            return !blocker && b == null;
        }
    }
}