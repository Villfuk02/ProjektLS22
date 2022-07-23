using System;
using System.Collections.Generic;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    /// <summary>
    /// Lets the user control the player.
    /// </summary>
    class HumanPlayerController : PlayerController
    {
        public static readonly string cardChoiceLetters = "QWERTASDFGHJ";
        public HumanPlayerController(Player p) : base(p, true) { }
        public override void GetOptions(Game.Phase phase, int step, Card? trumps, List<Card> trick)
        {
            _printer.P($"{_playerNames[Index]}: ");
            switch (phase)
            {
                case Game.Phase.BEGIN:
                    {
                        if (step == 1)
                        {
                            _printer.P("| Vyber trumf ");
                            Renderer.PrintValidChoices(Hand, trumps, trick, _TrumpValidator);
                            _printer.P(" | Vyber z ").H("Lidu |");
                        }
                        else if (step == 3)
                        {
                            _printer.P("| Odhoď do talonu ");
                            Renderer.PrintValidChoices(Hand, trumps, trick, _TalonValidator);
                            _printer.P(" |");
                        }
                        break;
                    }
                case Game.Phase.GAME:
                    {
                        _printer.P("| Zahraj kartu ");
                        Renderer.PrintValidChoices(Hand, trumps, trick, _PlayValidator);
                        _printer.P(" |");
                        break;
                    }
            }
        }

        public override Card? ChooseTrumps()
        {
            List<Card> sorted = new List<Card>(Hand.Enumerate());
            _SortCards(ref sorted, null);
            int choice = -1;
            do
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.L)
                    return null;
                choice = cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
            } while (choice == -1 || choice >= sorted.Count);
            return sorted[choice];
        }

        public override Card ChooseTalon(Card trumps, Pile talon)
        {
            List<Card> sorted = new List<Card>(Hand.Enumerate());
            _SortCards(ref sorted, trumps.Suit);
            int choice = -1;
            do
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                choice = cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
            } while (choice == -1 || choice >= sorted.Count);
            return sorted[choice];
        }
        public override Card ChoosePlay(List<Card> trick, Card trumps)
        {
            List<Card> sorted = new List<Card>(Hand.Enumerate());
            _SortCards(ref sorted, trumps.Suit);
            int choice = -1;
            do
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                choice = cardChoiceLetters.IndexOf(char.ToUpper(k.KeyChar));
            } while (choice == -1 || choice >= sorted.Count);
            return sorted[choice];
        }
    }
}