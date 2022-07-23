using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjektLS22
{
    /// <summary>
    /// For simpler printing to the console and formatting.
    /// </summary>
    public class Printer
    {
        public static Printer _printer = new Printer();
        /// <summary>
        /// Print a string.
        /// </summary>
        public Printer P(string s)
        {
            Console.Write(s);
            return this;
        }
        /// <summary>
        /// Print a char.
        /// </summary>
        public Printer P(char c)
        {
            Console.Write(c);
            return this;
        }
        /// <summary>
        /// Print a string, but padded or truncated to the given length.
        /// </summary>
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
        /// <summary>
        /// Print an int, padded or truncated to the given length.
        /// </summary>
        public Printer P(int n, int len)
        {
            string s = n.ToString();
            return P(s, len, false);
        }
        /// <summary>
        /// Print a number of spaces.
        /// </summary>
        public Printer S(int amt)
        {
            if (amt > 0)
                return P(new string(' ', amt));
            return this;
        }
        /// <summary>
        /// Change the background color.
        /// </summary>
        public Printer B(ConsoleColor c)
        {
            Console.BackgroundColor = c;
            return this;
        }
        /// <summary>
        /// Change the foreground color.
        /// </summary>
        public Printer F(ConsoleColor c)
        {
            Console.ForegroundColor = c;
            return this;
        }
        /// <summary>
        /// Reset foreground color to Gray and background to black.
        /// </summary>
        public Printer R()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }
        /// <summary>
        /// Print one or more newlines.
        /// </summary>
        public Printer NL(int count = 1)
        {
            Console.Write(new string('\n', count));
            return this;
        }
        /// <summary>
        /// Clear the console.
        /// </summary>
        public Printer CLR()
        {
            Console.Clear();
            return this;
        }
        /// <summary>
        /// Print a single digit, substituting 'X' for numbers 10 and up.
        /// </summary>
        public Printer D(int n)
        {
            if (n < 10)
                return P(n, 1);
            else
                return P('X');
        }
        /// <summary>
        /// Change the foreground color to DarkGray.
        /// </summary>
        public Printer DG()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            return this;
        }
        /// <summary>
        /// Change the foreground color to Gray.
        /// </summary>
        public Printer G()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            return this;
        }
        /// <summary>
        /// Change the foreground color to White.
        /// </summary>
        public Printer W()
        {
            Console.ForegroundColor = ConsoleColor.White;
            return this;
        }
        /// <summary>
        /// Change the background color to Black.
        /// </summary>
        public Printer B()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }
        /// <summary>
        /// Creates a highlight by swithcing the foreground and background colors.
        /// </summary>
        public Printer H()
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = c;
            return this;
        }
        /// <summary>
        /// Prints a string, highlighting the first character.
        /// </summary>
        public Printer H(string s)
        {
            return H().P(s[0]).H().P(s.Substring(1));
        }
        /// <summary>
        /// Prints a card.
        /// </summary>
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
        /// <summary>
        /// Prints a collection of cards if visible, otherwise the corresponding amount of card backs (<see cref="CB"/>).
        /// </summary>
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
        /// <summary>
        /// Print one or more card backs.
        /// </summary>
        public Printer CB(int count = 1)
        {
            ConsoleColor b = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Blue;
            S(count);
            Console.BackgroundColor = b;
            return this;
        }
        /// <summary>
        /// Prints a simple formatted string. Use *_* for white highlight and |_| for the <see cref="H"/> highlight.
        /// </summary>
        public Printer PF(string text)
        {
            string[] parts = text.Split('*');
            for (int i = 0; i < parts.Length; i++)
            {
                R();
                if (i % 2 == 1)
                    W();
                string[] highlights = parts[i].Split('|');
                for (int j = 0; j < highlights.Length; j++)
                {
                    P(highlights[j]).H();
                }
            }
            return R();
        }
    }
}