using System;
using System.Collections.Generic;


namespace ProjektLS22
{
    public class Renderer
    {
        public static readonly int WIDTH_PER_PLAYER = 15;
        public static Print PRINT = new Print();
        public static void RenderState(Game g)
        {
            PRINT.CLR();
            if (g.trumps != null)
            {
                PRINT.P("Trumfy: ").C(g.trumps);
                if (g.zLidu)
                    PRINT.DG().P("(z lidu)").R();
                PRINT.P(" | ");
            }
            PRINT.P(g.status).NL().NL();
            switch (g.phase)
            {
                case Game.Phase.CUT:
                    {
                        List<Card> lower = g.deck.GetRange(32 - g.info, g.info);
                        List<Card> upper = g.deck.GetRange(0, 32 - g.info);
                        if (g.step <= 1)
                        {
                            PRINT.C(lower, Hidden(g)).C(upper, Hidden(g)).NL(5);
                        }
                        else if (g.step == 2)
                        {
                            PRINT.C(lower, Hidden(g)).S(1).C(upper, Hidden(g)).NL(4);
                        }
                        else if (g.step == 3)
                        {
                            PRINT.S(lower.Count + 1).C(upper, Hidden(g)).NL().C(lower, Hidden(g)).NL(4);
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
                case Game.Phase.DEAL:
                    {
                        PRINT.C(g.deck, Hidden(g)).NL().NL();
                        PrintPlayers(g, true);
                        break;
                    }
                case Game.Phase.STAKES:
                    {
                        PRINT.NL();
                        // print flekování
                        PRINT.NL();
                        PrintPlayers(g, g.step == 1);
                        if (g.players[g.activePlayer].controller.isHuman)
                        {
                            if (g.step == 1)
                            {
                                PrintCardSelection(g, Utils.ValidTrump);
                            }
                            else if (g.step == 3)
                            {
                                PrintCardSelection(g, Utils.ValidTalon);
                            }
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        PRINT.NL(2);//print trick
                        PrintPlayers(g, false);
                        break;
                    }
            }
            if (g.waitingForPlayer)
            {
                PRINT.NL();
                g.players[g.activePlayer].controller.GetOptions(g);
            }
        }

        static bool Hidden(Game g)
        {
            return g.cardShowing == Game.CardShowing.ALL;
        }

        static bool ShowHand(Game g, int player)
        {
            if (g.cardShowing == Game.CardShowing.ALL)
                return true;
            if (g.cardShowing == Game.CardShowing.HUMAN)
                return g.players[player].controller.isHuman;
            if (g.cardShowing == Game.CardShowing.ACTIVE_HUMAN)
                return g.players[player].controller.isHuman && player == g.activePlayer;
            return false;
        }

        static void PrintPlayers(Game g, bool seven)
        {
            PRINT.G();
            for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
            {
                if (g.waitingForPlayer && g.activePlayer == i)
                {
                    PRINT.W().P($">{Utils.TranslatePlayer(i)}<", WIDTH_PER_PLAYER, true).G();
                }
                else
                {
                    PRINT.P($" {Utils.TranslatePlayer(i)}", WIDTH_PER_PLAYER, true);
                }
            }
            if (g.talon.Count > 0)
            {
                PRINT.P(" Talón");
            }
            PRINT.NL();
            for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
            {
                bool visible = ShowHand(g, i);
                PRINT.S(1);
                int width = PrintHand(g.players[i].hand, visible, seven && i == g.activePlayer, g.cardShowing == Game.CardShowing.ALL);
                PrintTricks(WIDTH_PER_PLAYER - 2 - width, 0, null);
                PRINT.S(1);
            }
            if (g.talon.Count > 0)
            {
                PRINT.S(1);
                PrintHand(g.talon, Hidden(g), false, g.cardShowing == Game.CardShowing.ALL);
            }
            PRINT.NL();
        }

        public static int PrintHand(List<Card> hand, bool visible, bool seven, bool all)
        {
            if (seven && hand.Count > 7)
            {
                PRINT.C(hand.GetRange(0, 7), visible).S(1).C(hand.GetRange(7, hand.Count - 7), all);
                return hand.Count + 1;
            }
            else
            {
                PRINT.C(hand, visible);
                return hand.Count;
            }
        }

        public static void PrintTricks(int space, int tricks, List<Card> marriages)
        {
            if (tricks > 0)
                PRINT.S(space - 1).B(ConsoleColor.Blue).W().D(tricks).R();
            else
                PRINT.S(space);
        }

        public static void PrintCardSelection(Game g, Func<int, Card, Card, List<Card>, bool> validator)
        {
            PRINT.S(1);
            for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
            {
                if (g.activePlayer == i)
                {
                    PRINT.DG();
                    for (int j = 0; j < WIDTH_PER_PLAYER; j++)
                    {
                        if (j < g.players[i].hand.Count && validator(j, g.players[i].hand[j], g.trumps, g.trick))
                        {
                            PRINT.P(HumanPlayerController.cardChoiceLetters[j]);
                        }
                        else
                        {
                            PRINT.S(1);
                        }
                    }
                    PRINT.R();
                }
                else
                {
                    PRINT.S(WIDTH_PER_PLAYER);
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
            //DIGIT
            public Print D(int n)
            {
                if (n < 10)
                    return P(n, 1);
                else
                    return P('X');
            }
            //DARK GRAY
            public Print DG()
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