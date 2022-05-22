using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class GameSetup
    {
        PlayerController.Type[] players = new PlayerController.Type[3];
        enum State { Menu, Rules, Finished };
        State state = State.Menu;
        InputHandler menuHandler = new InputHandler();
        InputHandler rulesHandler = new InputHandler();
        public static readonly int[] CASH_OPTIONS = new int[] { 20, 50, 100, 200, 500, 1000 };
        public int cash_selected = 2;
        bool fast = false;

        public GameSetup()
        {
            menuHandler.RegisterOption('S', () => { state = State.Finished; });
            menuHandler.RegisterOption('P', () => { state = State.Rules; });
            menuHandler.RegisterParametricOption("JFK", ChangeAI);
            //debug only
            menuHandler.RegisterOption('X', () => { fast = true; state = State.Finished; });

            rulesHandler.RegisterOption('S', () => { state = State.Finished; });
            rulesHandler.RegisterOption('Z', () => { state = State.Menu; });

            for (int i = 0; i < 3; i++)
            {
                players[i] = PlayerController.TYPES[i % PlayerController.TYPES.Count];
            }
            while (state != State.Finished)
            {
                PrintInfo();
                if (state == State.Menu)
                    menuHandler.ProcessInput();
                else if (state == State.Rules)
                    rulesHandler.ProcessInput();
            }
        }

        void PrintInfo()
        {
            if (state == State.Rules)
            {
                Renderer.PRINT.CLR().R().G().P("Tohle jsou pravidla.");
            }
            else
            {
                Renderer.PRINT.CLR().R().G().P("Nastavení hry:").NL().NL();
                for (int i = 0; i < 3; i++)
                {
                    Renderer.PRINT.W().P(" ").B(ConsoleColor.Blue).P(" ").P(Utils.TranslatePlayer(i), 7, true).B().P("  ");
                }
                Renderer.PRINT.NL();
                for (int i = 0; i < 3; i++)
                {
                    Renderer.PRINT.W().P(" ").B(ConsoleColor.Blue).P(players[i].label, 8, true).B().P("  ");
                }
            }
            Renderer.PRINT.NL().NL();
            switch (state)
            {
                case State.Menu:
                    Renderer.PRINT.G().P("| ").H("Start | Změň AI ").H("Jardy, ").H("Franty nebo ").H("Karla | ").H("Pravidla |");
                    break;
                case State.Rules:
                    Renderer.PRINT.G().P("| ").H("Start | ").H("Zpět do menu |");
                    break;
            }
            Renderer.PRINT.NL();
        }

        void ChangeAI(int player)
        {
            int a = (players[player].id + 1) % PlayerController.TYPES.Count;
            players[player] = PlayerController.TYPES[a];
        }

        public Game NewGame()
        {
            return new Game(players, fast);
        }
    }
}