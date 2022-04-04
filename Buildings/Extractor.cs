using System;

namespace ProjektLS22
{
    class Extractor : Building
    {
        static readonly int PRODUCTION = 3;
        public Extractor() : base('$', 25, $"Produces ${PRODUCTION} per turn per neigboring tile of your color.")
        {
        }

        public override void OnTurn(Player p, Tile t, World w)
        {
            ConsoleColor c = p.color;
            if (c == t.color)
            {
                foreach (Tile n in w.GetNeighbors(t))
                {
                    if (n.color == c)
                    {
                        p.cash += PRODUCTION;
                    }
                }
            }
        }
    }
}