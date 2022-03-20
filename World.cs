using System;

namespace ProjektLS22
{

    public class World
    {
        public int size;

        public Tile[,] tiles;
        public Player[] players;

        public World(int size, Player[] players)
        {
            this.size = size;
            this.players = players;

            tiles = new Tile[size + 2, size + 2];
            for (int x = 0; x < size + 2; x++)
            {
                for (int y = 0; y < size + 2; y++)
                {
                    tiles[x, y] = new Tile(this, new Pos(x, y), x == 0 || x > size || y == 0 || y > size);
                }
            }
        }
    }
}