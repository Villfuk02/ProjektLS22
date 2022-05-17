using System;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        public static readonly string cardChoiceLetters = "QWERTASDFGHJ";

        public HumanPlayerController(Player p) : base(p)
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
                            Renderer.PRINT.P("| Vyber trumf ").H().P(cardChoiceLetters.Substring(0, 7)).H().P(" | Vyber z ").H("Lidu |");
                        }
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
    }
}