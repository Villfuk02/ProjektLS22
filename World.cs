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
    }
}