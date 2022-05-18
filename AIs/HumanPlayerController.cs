using System;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        public static readonly string cardChoiceLetters = "QWERTASDFGHJ";

        public HumanPlayerController()
        {
            isHuman = true;
        }
        public override void GetOptions(Game g)
        {
            Renderer.PRINT.P($"{Utils.TranslatePlayer(g.activePlayer)}: ");
            switch (g.phase)
            {
                case Game.Phase.STAKES:
                    {
                        if (g.step == 1)
                        {
                            Renderer.PRINT.P("| Vyber trumf ");
                            Utils.PrintValidChoices(player.hand, g.trumps, g.trick, Utils.ValidTrump);
                            Renderer.PRINT.P(" | Vyber z ").H("Lidu |");
                        }
                        else if (g.step == 3)
                        {
                            Renderer.PRINT.P("| Odhoď do talonu ");
                            Utils.PrintValidChoices(player.hand, g.trumps, g.trick, Utils.ValidTalon);
                            Renderer.PRINT.P(" |");
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        Renderer.PRINT.P("| Zahraj kartu ");
                        Utils.PrintValidChoices(player.hand, g.trumps, g.trick, Utils.ValidPlay);
                        Renderer.PRINT.P(" |");
                        break;
                    }
            }
        }

        public override int ChooseTrumps(Game g)
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.L)
                return -1;
            int choice = cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
            if (choice == -1 || choice >= 7)
                return -2;
            return choice;
        }

        public override int ChooseTalon(Game g)
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            return cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
        }
        public override int ChoosePlay(Game g)
        {
            ConsoleKeyInfo k = Console.ReadKey(true);
            return cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
        }
    }
}