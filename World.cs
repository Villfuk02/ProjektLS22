using System;

namespace ProjektLS22
{

    public class World
    {
        static Random rand = new Random();
        public int size;

        Tile[,] tiles;
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

            double center = (size + 1) * 0.5;
            double offset = size * 0.4;
            double variance = size * 0;
            double angle = rand.NextDouble() * 2 * Math.PI;
            for (int i = 0; i < players.Length; i++)
            {
                angle += 2 * Math.PI / players.Length;
                (double x, double y) = Utils.OnCircle(offset, angle);
                (double ox, double oy) = Utils.GetRandomPointInsideCircle(variance);
                Pos pos = new Pos((int)Math.Round(center + x + ox), (int)Math.Round(center + y + oy));
                tiles[pos.x, pos.y].color = players[i].color;
                foreach (Pos p in Pos.NEIGHBORS)
                {
                    tiles[pos.x + p.x, pos.y + p.y].color = players[i].color;
                }
                tiles[pos.x, pos.y].Place(players[i], this, new Base(players[i]));
                players[i].basePos = pos;
            }
        }

        public void TakeTurn(int activePlayer)
        {
            Renderer.PRINT.R().H().P("DO SOMETHING").R().W().NL();
            while (true)
            {
                string error = TryAction(players[activePlayer].GetIntent(this));
                if (error == "")
                    break;
                Renderer.PRINT.CL(2).R().F(ConsoleColor.Red).P(error).R().W().NL();
            }
        }

        public string TryAction(Player.Action a)
        {
            return "ERRRRRRR";
        }

        public Tile GetTile(Pos p)
        {
            return tiles[p.x, p.y];
        }
        public Tile[] GetNeighbors(Tile t)
        {
            Tile[] ret = new Tile[Pos.NEIGHBORS.Length];
            for (int i = 0; i < Pos.NEIGHBORS.Length; i++)
            {
                ret[i] = GetTile(t.p + Pos.NEIGHBORS[i]);
            }
            return ret;
        }
    }
}