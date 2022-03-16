using System;

namespace ProjektLS22
{
    class Program
    {
        static World w;
        static void Main(string[] args)
        {
            w = new World(20);
            Renderer.RenderState(w);
            Console.ReadLine();
        }
    }
}
