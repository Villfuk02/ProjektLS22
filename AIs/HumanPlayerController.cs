using System;
using System.Collections.Generic;

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
            Renderer.PRINT.P($"{Utils.TranslatePlayer(player.index)}: ");
            switch (phase)
            {
                case Game.Phase.STAKES:
                    {
                        if (step == 1)
                        {
                            Renderer.PRINT.P("| Vyber trumf ");
                            Utils.PrintValidChoices(player.hand, trumps, trick, Utils.ValidTrump);
                            Renderer.PRINT.P(" | Vyber z ").H("Lidu |");
                        }
                        else if (step == 3)
                        {
                            Renderer.PRINT.P("| Odhoď do talonu ");
                            Utils.PrintValidChoices(player.hand, trumps, trick, Utils.ValidTalon);
                            Renderer.PRINT.P(" |");
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        Renderer.PRINT.P("| Zahraj kartu ");
                        Utils.PrintValidChoices(player.hand, trumps, trick, Utils.ValidPlay);
                        Renderer.PRINT.P(" |");
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