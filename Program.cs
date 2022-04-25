using System;

namespace ProjektLS22
{
    class Program
    {
        static Game g;
        static int turn = 1;
        static int activePlayer = 0;
        static void Main(string[] args)
        {
            while (true)
            {
                Init();
                while (g.active)
                {
                    Renderer.RenderState(g);
                    g.NextStep();
                }
            }
        }

        static void Init()
        {
            g = new GameSetup().NewGame();
        }
    }
}
