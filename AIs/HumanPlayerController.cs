using System;
using System.Collections.Generic;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        public static readonly string cardChoiceLetters = "QWERTASDFGHJ";

        public HumanPlayerController()
        {
            isHuman = true;
        }
        public override void GetOptions(Game.Phase phase, int step, Card trumps, List<Card> trick)
        {
            _printer.P($"{_playerNames[player.index]}: ");
            switch (phase)
            {
                case Game.Phase.BEGIN:
                    {
                        if (step == 1)
                        {
                            _printer.P("| Vyber trumf ");
                            Renderer.PrintValidChoices(player.hand, trumps, trick, _ValidTrump);
                            _printer.P(" | Vyber z ").H("Lidu |");
                        }
                        else if (step == 3)
                        {
                            _printer.P("| Odhoď do talonu ");
                            Renderer.PrintValidChoices(player.hand, trumps, trick, _ValidTalon);
                            _printer.P(" |");
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        _printer.P("| Zahraj kartu ");
                        Renderer.PrintValidChoices(player.hand, trumps, trick, _ValidPlay);
                        _printer.P(" |");
                        break;
                    }
            }
        }

        public override int ChooseTrumps()
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.L)
                return -1;
            int choice = cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
            if (choice == -1 || choice >= 7)
                return -2;
            return choice;
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            return cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            return cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
        }
    }
}