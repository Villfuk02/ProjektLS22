using System;
using System.Collections.Generic;


namespace ProjektLS22
{
    public class Renderer
    {
        public static readonly int WIDTH_PER_PLAYER = 16;
        public static Print PRINT = new Print();
        public static void RenderState(Game g)
        {
            PRINT.CLR().R();
            if (g.trumps != null)
            {
                PRINT.P("Trumfy: ").C(g.trumps);
                if (g.zLidu)
                    PRINT.DG().P("(z lidu)").R();
                PRINT.P(" | ");
            }
            if (g.phase != Game.Phase.SCORE)
                PRINT.P(g.status);
            PRINT.NL().NL();
            switch (g.phase)
            {
                case Game.Phase.CUT:
                    {
                        List<Card> lower = g.deck.GetRange(32 - g.info, g.info);
                        List<Card> upper = g.deck.GetRange(0, 32 - g.info);
                        if (g.step <= 1)
                        {
                            PRINT.C(lower, Hidden(g)).C(upper, Hidden(g)).NL(2);
                        }
                        else if (g.step == 2)
                        {
                            PRINT.C(lower, Hidden(g)).S(1).C(upper, Hidden(g)).NL(2);
                        }
                        else if (g.step == 3)
                        {
                            PRINT.S(lower.Count + 1).C(upper, Hidden(g)).NL().C(lower, Hidden(g)).NL();
                        }
                        else if (g.step == 4)
                        {
                            PRINT.C(upper, Hidden(g)).NL().S(upper.Count + 1).C(lower, Hidden(g)).NL();
                        }
                        else if (g.step == 5)
                        {
                            PRINT.C(upper, Hidden(g)).S(1).C(lower, Hidden(g)).NL(2);
                        }
                        else if (g.step == 6)
                        {
                            PRINT.C(upper, Hidden(g)).C(lower, Hidden(g)).NL(2);
                        }
                        PrintPlayers(g, false);
                        break;
                    }
                case Game.Phase.DEAL:
                    {
                        PRINT.C(g.deck, Hidden(g)).NL(2);
                        PrintPlayers(g, true);
                        break;
                    }
                case Game.Phase.STAKES:
                    {
                        PRINT.NL(2);
                        PrintPlayers(g, g.step <= 1);
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
                        PrintTrick(g.trick, g.activePlayer);
                        PRINT.NL();
                        PrintPlayers(g, false);
                        if (g.step == 1)
                        {
                            PrintCardSelection(g, Utils.ValidPlay);
                        }
                        break;
                    }
                case Game.Phase.SCORE:
                    {
                        PRINT.S(9).F(ConsoleColor.Green).P(g.status);
                        PRINT.NL(2);
                        PrintPlayers(g, false);
                        break;
                    }
                case Game.Phase.COLLECT:
                    {
                        PRINT.C(g.deck, Hidden(g)).NL(2);
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
                return g.players[player].controller.isHuman && player == g.activePlayer && g.waitingForPlayer;
            return false;
        }

        static void PrintPlayers(Game g, bool seven)
        {
            for (int i = 0; i < 3; i++)
            {
                PRINT.R();
                bool offense = (g.dealer + 1) % 3 == i;
                if (offense)
                    PRINT.F(ConsoleColor.Red);
                if (g.waitingForPlayer && g.activePlayer == i)
                {
                    if (!offense)
                        PRINT.W();
                    PRINT.P($">{Utils.TranslatePlayer(i)}< {g.players[i].score}", WIDTH_PER_PLAYER, true);
                }
                else
                {
                    PRINT.P($" {Utils.TranslatePlayer(i)}  {g.players[i].score}", WIDTH_PER_PLAYER, true);
                }
            }
            if (g.talon.Count > 0)
            {
                PRINT.R().P(" Talon");
            }
            PRINT.NL();
            for (int i = 0; i < 3; i++)
            {
                bool visible = ShowHand(g, i);
                PRINT.S(1);
                int width = PrintHand(g.players[i].hand, visible, seven && i == g.activePlayer, g.cardShowing == Game.CardShowing.ALL);
                PrintTricks(WIDTH_PER_PLAYER - 3 - width, g.players[i].discard.Count / 3, g.players[i].marriages);
                PRINT.S(2);
            }
            if (g.talon.Count > 0)
            {
                PRINT.S(1);
                PrintHand(g.talon, Hidden(g), false);
            }
            PRINT.NL();
        }

        public static int PrintHand(List<Card> hand, bool visible, bool seven, bool all = false)
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
            PRINT.S(space - marriages.Count - 1);
            PrintHand(marriages, true, false);
            if (tricks > 0)
            {
                PRINT.B(ConsoleColor.Blue).W().D(tricks).R();
            }
            else
            {
                PRINT.S(1);
            }
        }

        public static void PrintCardSelection(Game g, Func<List<Card>, int, Card, List<Card>, bool> validator)
        {
            PRINT.S(1);
            for (int i = 0; i < 3; i++)
            {
                if (g.activePlayer == i && g.players[i].controller.isHuman)
                {
                    PRINT.DG();
                    for (int j = 0; j < WIDTH_PER_PLAYER; j++)
                    {
                        if (j < g.players[i].hand.Count && validator(g.players[i].hand, j, g.trumps, g.trick))
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

        public static void PrintTrick(List<Card> trick, int activePlayer)
        {
            Card[] playerCards = new Card[3];
            for (int i = 0; i < trick.Count; i++)
            {
                playerCards[(activePlayer - trick.Count + 3 + i) % 3] = trick[i];
            }
            PRINT.S(14);
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null)
                    PRINT.C(playerCards[i]).S(6);
                else
                    PRINT.S(7);
            }
            PRINT.NL();
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
                if (amt > 0)
                    return P(new string(' ', amt));
                return this;
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