using System;

namespace ProjektLS22
{
    class Program
    {
        static World w;
        static int turn = 1;
        static int activePlayer = 0;
        static void Main(string[] args)
        {
            Init();
            while (true)
            {
                Renderer.RenderState(w);
                TakeTurn();
                activePlayer++;
                if (activePlayer >= w.players.Length)
                {
                    activePlayer = 0;
                    turn++;
                }
            }
        }

        static void Init()
        {
            w = new GameSetup().InitWorld();
        }

        static void TakeTurn()
        {
            w.TakeTurn(activePlayer);
        }
    }
}
