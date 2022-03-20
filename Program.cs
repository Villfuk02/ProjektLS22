using System;
using System.Collections.Generic;

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
            int playerAmt = ReadIntInRange("Select number of players", 2, 6);
            int worldSIze = ReadIntInRange("Select board size", 10, 30);
            ConsoleColor[] colors = new ConsoleColor[playerAmt];
            ConsoleColor[] availableColors = new ConsoleColor[] {
                ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta, ConsoleColor.DarkCyan, ConsoleColor.DarkYellow};
            for (int i = 0; i < playerAmt; i++)
            {
                colors[i] = availableColors[i];
            }
            w = new World(worldSIze, colors);
        }

        static int ReadIntInRange(string prompt, int min, int max)
        {
            Int32 p = min - 1;
            while (p < min || p > max)
            {
                Console.WriteLine($"{prompt}({min} - {max}):");
                string s = Console.ReadLine();
                Int32.TryParse(s, out p);
            }
            return p;
        }
    }
}
