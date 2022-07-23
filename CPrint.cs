using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class Printer
    {
        public static Printer _printer = new Printer();
        //PRINT
        public Printer P(string s)
        {
            Console.Write(s);

            return this;
        }
        public Printer P(char c)
        {
            Console.Write(c);
            return this;
        }
        public Printer P(string s, int len, bool alignLeft)
        {
            if (s.Length > len)
                return P(s.Substring(0, len));
            if (s.Length == len)
                return P(s);
            if (alignLeft)
                return P(s).S(len - s.Length);
            return S(len - s.Length).P(s);
        }
        public Printer P(int n, int len)
        {
            string s = n.ToString();
            return P(s, len, false);
        }
        //SPACING
        public Printer S(int amt)
        {
            if (amt > 0)
                return P(new string(' ', amt));
            return this;
        }
        //BACKGROUND COLOR
        public Printer B(ConsoleColor c)
        {
            Console.BackgroundColor = c;
            return this;
        }
        //FOREGROUND COLOR
        public Printer F(ConsoleColor c)
        {
            Console.ForegroundColor = c;
            return this;
        }
        //RESET COLORS
        public Printer R()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }
        //NEW LINE
        public Printer NL(int count = 1)
        {
            Console.Write(new string('\n', count));
            return this;
        }
        //CLEAR
        public Printer CLR()
        {
            Console.Clear();
            return this;
        }
        //DIGIT
        public Printer D(int n)
        {
            if (n < 10)
                return P(n, 1);
            else
                return P('X');
        }
        //DARK GRAY
        public Printer DG()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            return this;
        }
        //GRAY
        public Printer G()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            return this;
        }
        //WHITE
        public Printer W()
        {
            Console.ForegroundColor = ConsoleColor.White;
            return this;
        }
        //BLACK BACKGROUND
        public Printer B()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }
        //HIGHLIGHT - SWITCH COLORS
        public Printer H()
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = c;
            return this;
        }
        public Printer H(string s)
        {
            return H().P(s[0]).H().P(s.Substring(1));
        }
        //CLEAR LINE
        public Printer CL(int lines)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop - lines + 1);
            S(Console.WindowWidth - 1);
            Console.SetCursorPosition(0, currentLineCursor - lines + 1);
            return this;
        }
        //PRINT CARD
        public Printer C(Card card)
        {
            ConsoleColor f = Console.ForegroundColor;
            ConsoleColor b = Console.BackgroundColor;
            Console.BackgroundColor = card.Suit.Color;
            Console.ForegroundColor = ConsoleColor.Black;
            P(card.Value.Symbol);
            Console.BackgroundColor = b;
            Console.ForegroundColor = f;
            return this;
        }
        public Printer C(List<Card> cards, bool show)
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
                    Console.BackgroundColor = c.Suit.Color;
                    Console.ForegroundColor = ConsoleColor.Black;
                    P(c.Value.Symbol);
                }
                Console.BackgroundColor = b;
                Console.ForegroundColor = f;
                return this;
            }
        }
        //PRINT CARD BACKS
        public Printer CB(int count = 1)
        {
            ConsoleColor b = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Blue;
            S(count);
            Console.BackgroundColor = b;
            return this;
        }
    }
}