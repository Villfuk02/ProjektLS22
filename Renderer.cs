using System;

namespace ProjektLS22
{
    public class Renderer
    {
        public static Print PRINT = new Print();
        public static void RenderState(Game g, string status)
        {
            PRINT.CLR();
            PRINT.P(status).NL();
            switch (g.phase)
            {
                case Game.Phase.CUT:
                    {
                        PRINT.CB(32).NL();
                        break;
                    }
            }
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
            public Print CLR()
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
            //PRINT CARD
            public Print C(Card card)
            {
                ConsoleColor f = Console.ForegroundColor;
                ConsoleColor b = Console.BackgroundColor;
                Console.BackgroundColor = card.suit.color;
                Console.ForegroundColor = ConsoleColor.Black;
                P(card.value.symbol);
                Console.BackgroundColor = b;
                Console.ForegroundColor = f;
                return this;
            }
            //PRINT CARD BACKS
            public Print CB(int count = 1)
            {
                ConsoleColor b = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Blue;
                S(count);
                Console.BackgroundColor = b;
                return this;
            }
        }
    }
}