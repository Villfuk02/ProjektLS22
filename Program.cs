using System;

namespace ProjektLS22
{
    class Program
    {
        static Game g;
        static void Main(string[] args)
        {
            while (true)
            {
                Init();
                while (g.simulate != 0)
                {
                    g.NextStep();
                }
                Console.ReadLine();
            }
        }

        static void Init()
        {
            g = new GameSetup().NewGame();
        }
    }
}
