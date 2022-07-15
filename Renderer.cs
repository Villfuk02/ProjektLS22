using System;
using System.Collections.Generic;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    public class Renderer
    {
        public static readonly int WIDTH_PER_PLAYER = 16;
        public static void RenderState(Game g)
        {
            _printer.CLR().R();
            if (g.trumps != null)
            {
                _printer.P("Trumfy: ").C(g.trumps);
                if (g.fromPeople)
                    _printer.DG().P("(z lidu)").R();
                _printer.P(" | ");
            }
            if (g.phase != Game.Phase.SCORE)
                _printer.P(g.status);
            _printer.NL().NL();
            switch (g.phase)
            {
                case Game.Phase.CUT:
                    {
                        List<Card> lower = g.deck.GetRange(32 - g.info, g.info);
                        List<Card> upper = g.deck.GetRange(0, 32 - g.info);
                        if (g.step <= 1)
                        {
                            _printer.C(lower, Hidden(g)).C(upper, Hidden(g)).NL(2);
                        }
                        else if (g.step == 2)
                        {
                            _printer.C(lower, Hidden(g)).S(1).C(upper, Hidden(g)).NL(2);
                        }
                        else if (g.step == 3)
                        {
                            _printer.S(lower.Count + 1).C(upper, Hidden(g)).NL().C(lower, Hidden(g)).NL();
                        }
                        else if (g.step == 4)
                        {
                            _printer.C(upper, Hidden(g)).NL().S(upper.Count + 1).C(lower, Hidden(g)).NL();
                        }
                        else if (g.step == 5)
                        {
                            _printer.C(upper, Hidden(g)).S(1).C(lower, Hidden(g)).NL(2);
                        }
                        else if (g.step == 6)
                        {
                            _printer.C(upper, Hidden(g)).C(lower, Hidden(g)).NL(2);
                        }
                        PrintPlayers(g, false);
                        break;
                    }
                case Game.Phase.DEAL:
                    {
                        _printer.C(g.deck, Hidden(g)).NL(2);
                        PrintPlayers(g, true);
                        break;
                    }
                case Game.Phase.BEGIN:
                    {
                        _printer.NL(2);
                        PrintPlayers(g, g.step <= 1);
                        if (g.players[g.activePlayer].controller.isHuman)
                        {
                            if (g.step == 1)
                            {
                                PrintCardSelection(g, _ValidTrump);
                            }
                            else if (g.step == 3)
                            {
                                PrintCardSelection(g, _ValidTalon);
                            }
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        PrintTrick(g.trick, g.activePlayer);
                        _printer.NL();
                        PrintPlayers(g, false);
                        if (g.step == 1)
                        {
                            PrintCardSelection(g, _ValidPlay);
                        }
                        break;
                    }
                case Game.Phase.SCORE:
                    {
                        _printer.S(9).F(ConsoleColor.Green).P(g.status);
                        _printer.NL(2);
                        PrintPlayers(g, false);
                        break;
                    }
                case Game.Phase.COLLECT:
                    {
                        _printer.C(g.deck, Hidden(g)).NL(2);
                        PrintPlayers(g, false);
                        break;
                    }
            }
            if (g.waitingForPlayer)
            {
                _printer.R().NL();
                g.players[g.activePlayer].controller.GetOptions(g.phase, g.step, g.trumps, g.trick);
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
                _printer.R();
                Player p = g.players[i];
                bool offense = _PPlus(g.dealer, 1) == i;
                if (offense)
                    _printer.F(ConsoleColor.Red);
                if (g.waitingForPlayer && g.activePlayer == i)
                {
                    if (!offense)
                        _printer.W();
                    _printer.P($">{_playerNames[i]}< {p.offense_wins + p.defense_wins}", WIDTH_PER_PLAYER, true);
                }
                else
                {
                    _printer.P($" {_playerNames[i]}  {p.offense_wins + p.defense_wins}", WIDTH_PER_PLAYER, true);
                }
            }
            if (g.talon.Count > 0)
            {
                _printer.R().P(" Talon");
            }
            _printer.NL();
            for (int i = 0; i < 3; i++)
            {
                bool visible = ShowHand(g, i);
                _printer.S(1);
                int width = PrintHand(g.players[i].hand, visible, seven && i == g.activePlayer, g.cardShowing == Game.CardShowing.ALL);
                PrintTricks(WIDTH_PER_PLAYER - 3 - width, g.players[i].discard.Count / 3, g.players[i].marriages);
                _printer.S(2);
            }
            if (g.talon.Count > 0)
            {
                _printer.S(1);
                PrintHand(g.talon, Hidden(g), false);
            }
            _printer.NL();
        }

        public static int PrintHand(List<Card> hand, bool visible, bool seven, bool all = false)
        {
            if (seven && hand.Count > 7)
            {
                _printer.C(hand.GetRange(0, 7), visible).S(1).C(hand.GetRange(7, hand.Count - 7), all);
                return hand.Count + 1;
            }
            else
            {
                _printer.C(hand, visible);
                return hand.Count;
            }
        }

        public static void PrintTricks(int space, int tricks, List<Card> marriages)
        {
            _printer.S(space - marriages.Count - 1);
            PrintHand(marriages, true, false);
            if (tricks > 0)
            {
                _printer.B(ConsoleColor.Blue).W().D(tricks).R();
            }
            else
            {
                _printer.S(1);
            }
        }

        public static void PrintCardSelection(Game g, Func<List<Card>, int, Card, List<Card>, bool> validator)
        {
            _printer.S(1);
            for (int i = 0; i < 3; i++)
            {
                if (g.activePlayer == i && g.players[i].controller.isHuman)
                {
                    _printer.DG();
                    for (int j = 0; j < WIDTH_PER_PLAYER; j++)
                    {
                        if (j < g.players[i].hand.Count && validator(g.players[i].hand, j, g.trumps, g.trick))
                        {
                            _printer.P(HumanPlayerController.cardChoiceLetters[j]);
                        }
                        else
                        {
                            _printer.S(1);
                        }
                    }
                    _printer.R();
                }
                else
                {
                    _printer.S(WIDTH_PER_PLAYER);
                }
            }
        }

        public static void PrintTrick(List<Card> trick, int activePlayer)
        {
            Card[] playerCards = new Card[3];
            for (int i = 0; i < trick.Count; i++)
            {
                playerCards[_PPlus(activePlayer, 3 - trick.Count + i)] = trick[i];
            }
            _printer.S(14);
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null)
                    _printer.C(playerCards[i]).S(6);
                else
                    _printer.S(7);
            }
            _printer.NL();
        }
        public static void PrintValidChoices(List<Card> cards, Card trumps, List<Card> trick, Func<List<Card>, int, Card, List<Card>, bool> validator)
        {
            _printer.H();
            for (int i = 0; i < cards.Count; i++)
            {
                if (validator(cards, i, trumps, trick))
                {
                    _printer.P(HumanPlayerController.cardChoiceLetters[i]);
                }
            }
            _printer.H();
        }
    }
}