using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
            if (g.trumps.HasValue)
            {
                _printer.P("Trumfy: ").C(g.trumps.Value);
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
                        if (g.players[g.activePlayer].controller.IsHuman)
                        {
                            if (g.step == 1)
                            {
                                PrintCardSelection(g, _TrumpValidator);
                            }
                            else if (g.step == 3)
                            {
                                PrintCardSelection(g, _TalonValidator);
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
                            PrintCardSelection(g, _PlayValidator);
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
                return g.players[player].controller.IsHuman;
            if (g.cardShowing == Game.CardShowing.ACTIVE_HUMAN)
                return g.players[player].controller.IsHuman && player == g.activePlayer && g.waitingForPlayer;
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
                _printer.S(1).C(g.players[i].hand, visible);
                int disCount = g.players[i].discard.Count;
                if (disCount % 3 == 0)
                    PrintDiscard(WIDTH_PER_PLAYER - 3 - g.players[i].hand.Count, disCount / 3, g.players[i].marriages);
                else
                    _printer.S(1).CB(5);
                _printer.S(2);
            }
            if (g.talon.Count > 0)
            {
                _printer.S(1).C(g.talon, Hidden(g));
            }
            _printer.NL();
        }

        public static void PrintDiscard(int space, int tricks, List<Card> marriages)
        {
            _printer.S(space - marriages.Count - 1).C(marriages, true);
            if (tricks > 0)
            {
                _printer.B(ConsoleColor.Blue).W().D(tricks).R();
            }
            else
            {
                _printer.S(1);
            }
        }

        public static void PrintCardSelection(Game g, Func<Pile, Card, Card?, List<Card>, bool> validator)
        {
            _printer.S(1);
            for (int i = 0; i < 3; i++)
            {
                if (g.activePlayer == i && g.players[i].controller.IsHuman)
                {
                    Pile h = new Pile(g.players[i].hand);
                    _printer.DG();
                    for (int j = 0; j < g.players[i].hand.Count; j++)
                    {
                        if (validator(h, g.players[i].hand[j], g.trumps, g.trick))
                        {
                            _printer.P(HumanPlayerController.cardChoiceLetters[j]);
                        }
                        else
                        {
                            _printer.S(1);
                        }
                    }
                    _printer.R().S(WIDTH_PER_PLAYER - g.players[i].hand.Count);
                }
                else
                {
                    _printer.S(WIDTH_PER_PLAYER);
                }
            }
        }

        public static void PrintTrick(List<Card> trick, int activePlayer)
        {
            Card?[] playerCards = new Card?[3];
            for (int i = 0; i < trick.Count; i++)
            {
                playerCards[_PPlus(activePlayer, 3 - trick.Count + i)] = trick[i];
            }
            _printer.S(14);
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i].HasValue)
                    _printer.C(playerCards[i].Value).S(6);
                else
                    _printer.S(7);
            }
            _printer.NL();
        }
        public static void PrintValidChoices(Pile hand, Card? trumps, List<Card> trick, Func<Pile, Card, Card?, List<Card>, bool> validator)
        {
            _printer.H();
            List<Card> sorted = new List<Card>(hand.Enumerate());
            _SortCards(ref sorted, trumps.HasValue ? trumps.Value.Suit : null);
            for (int i = 0; i < hand.Count; i++)
            {
                if (validator(hand, sorted[i], trumps, trick))
                {
                    _printer.P(HumanPlayerController.cardChoiceLetters[i]);
                }
            }
            _printer.H();
        }
    }
}