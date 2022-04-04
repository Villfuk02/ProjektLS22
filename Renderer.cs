using System;

namespace ProjektLS22
{
    public class Renderer
    {
        public static Print PRINT = new Print();
        static readonly Pos mapOffset = new Pos(3, 1);
        public static void RenderState(World w)
        {
            Console.Clear();
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
                    PrintTile(w, new Pos(x, y), false);
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

        static void PrintTile(World w, Pos pos, bool selected)
        {
            if (selected)
            {
                Console.Write("#");
            }
            else
            {
                Tile t = w.GetTile(pos);
                Console.BackgroundColor = t.color;
                if (t.b == null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write((pos.x % 5 == 0 || pos.y % 5 == 0) ? "." : " ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(t.b.symbol);
                }
            }
        }

        public static void UnpaintCursor(World w, Pos p)
        {
            Pos c = new Pos(Console.CursorLeft, Console.CursorTop);
            Pos cursor = p + mapOffset;
            Console.SetCursorPosition(cursor.x, cursor.y);
            PrintTile(w, p, false);
            Console.SetCursorPosition(c.x, c.y);
        }
        public static void PaintCursor(World w, Pos p)
        {
            Pos c = new Pos(Console.CursorLeft, Console.CursorTop);
            Pos cursor = p + mapOffset;
            Console.SetCursorPosition(cursor.x, cursor.y);
            PrintTile(w, p, true);
            Console.SetCursorPosition(c.x, c.y);
        }

        public class Print
        {
            public Print() { }
            //PRINT
            public Print P(string s)
            {
                Console.Write(s);
                return this;
            }
            public Print P(char c)
            {
                Console.Write(c);
                return this;
            }
            public Print P(string s, int len, bool alignLeft)
            {
                if (s.Length > len)
                    return P(s.Substring(0, len));
                if (s.Length == len)
                    return P(s);
                if (alignLeft)
                    return P(s).S(len - s.Length);
                return S(len - s.Length).P(s);
            }
            public Print P(int n, int len)
            {
                string s = n.ToString();
                return P(s, len, false);
            }
            //SPACING
            public Print S(int amt)
            {
                return P(new string(' ', amt));
            }
            //BACKGROUND COLOR
            public Print B(ConsoleColor c)
            {
                Console.BackgroundColor = c;
                return this;
            }
            //FOREGROUND COLOR
            public Print F(ConsoleColor c)
            {
                Console.ForegroundColor = c;
                return this;
            }
            //RESET COLORS
            public Print R()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                return this;
            }
            //NEW LINE
            public Print NL()
            {
                Console.Write('\n');
                return this;
            }
            //CLEAR
            public Print C()
            {
                Console.Clear();
                return this;
            }
            //DARK GRAY
            public Print D()
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                return this;
            }
            //GRAY
            public Print G()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                return this;
            }
            //WHITE
            public Print W()
            {
                Console.ForegroundColor = ConsoleColor.White;
                return this;
            }
            //BLACK BACKGROUND
            public Print B()
            {
                Console.BackgroundColor = ConsoleColor.Black;
                return this;
            }
            //HIGHLIGHT - SWITCH COLORS
            public Print H()
            {
                ConsoleColor c = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.BackgroundColor = c;
                return this;
            }
            public Print H(string s)
            {
                return H().P(s[0]).H().P(s.Substring(1));
            }
            //CLEAR LINE
            public Print CL(int lines)
            {
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop - lines + 1);
                S(Console.WindowWidth - 1);
                Console.SetCursorPosition(0, currentLineCursor - lines + 1);
                return this;
            }
        }
    }
}