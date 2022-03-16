using System;

namespace ProjektLS22
{
    public class Renderer
    {
        public static void RenderState(World w)
        {
            Console.Write("   ");
            for (int x = 0; x < w.size + 2; x++)
            {
                PrintAxisChar(x);
            }
            Console.Write("\n");
            for (int y = 0; y < w.size + 2; y++)
            {
                if (y % 5 == 0)
                {
                    PrintAxisChar(y - 2);
                    PrintAxisChar(y - 1);
                    PrintAxisChar(y);
                }
                else
                {
                    Console.Write("   ");
                }

                for (int x = 0; x < w.size + 2; x++)
                {
                    Console.BackgroundColor = w.tiles[x, y].color;
                    Console.Write((x % 5 == 0 || y % 5 == 0) ? "." : " ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("\n");
            }
        }

        static void PrintAxisChar(int pos)
        {
            int e = (1000 - pos) % 5;
            int d = 1;
            for (int i = 0; i < e; i++)
            {
                d *= 10;
            }
            if ((pos + e) / d == 0)
            {
                Console.Write(" ");
            }
            else
            {
                Console.Write((pos + e) / d % 10);
            }
        }
    }
}