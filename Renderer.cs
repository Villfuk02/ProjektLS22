using System;
using System.Collections.Generic;


namespace ProjektLS22
{
    public class Renderer
    {
        public static Print PRINT = new Print();
        public static void RenderState(Game g)
        {
            PRINT.CLR();
            PRINT.P(g.status).NL().NL();
            switch (g.phase)
            {
                case Game.Phase.CUT:
                    {
                        List<Card> lower = g.deck.GetRange(32 - g.info, g.info);
                        List<Card> upper = g.deck.GetRange(0, 32 - g.info);
                        if (g.step <= 1)
                        {
                            PRINT.C(lower, Hidden(g)).C(upper, Hidden(g)).NL(4);
                        }
                        else if (g.step == 2)
                        {
                            PRINT.C(lower, Hidden(g)).S(1).C(upper, Hidden(g)).NL(4);
                        }
                        else if (g.step == 3)
                        {
                            PRINT.S(lower.Count + 1).C(upper, Hidden(g)).NL().C(lower, Hidden(g)).NL(3);
                        }
                        else if (g.step == 4)
                        {
                            PRINT.C(upper, Hidden(g)).NL().S(upper.Count + 1).C(lower, Hidden(g)).NL(3);
                        }
                        else if (g.step == 5)
                        {
                            PRINT.C(upper, Hidden(g)).S(1).C(lower, Hidden(g)).NL(4);
                        }
                        else if (g.step == 6)
                        {
                            PRINT.C(upper, Hidden(g)).C(lower, Hidden(g)).NL(4);
                        }
                        break;
                    }
            }
        }

        static bool Hidden(Game g)
        {
            return g.cardShowing == Game.CardShowing.ALL;
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
            public Print NL(int count = 1)
            {
                Console.Write(new string('\n', count));
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
            public Print C(List<Card> cards, bool show)
            {
                if (!show)
                {
                    return CB(cards.Count);
                }
                else
                {
                    ConsoleColor f = Console.ForegroundColor;
                    ConsoleColor b = Console.BackgroundColor;
                    foreach (Card c in cards)
                    {
                        Console.BackgroundColor = c.suit.color;
                        Console.ForegroundColor = ConsoleColor.Black;
                        P(c.value.symbol);
                    }
                    Console.BackgroundColor = b;
                    Console.ForegroundColor = f;
                    return this;
                }
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