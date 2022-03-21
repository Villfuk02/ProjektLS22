using System;

namespace ProjektLS22
{
    class Program
    {
        static World w;
        static void Main(string[] args)
        {
            Init();
            Renderer.RenderState(w);
            Console.ReadLine();
        }

        static void Init()
        {
            w = new GameSetup().InitWorld();
        }
    }
}
