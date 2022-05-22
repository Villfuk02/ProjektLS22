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
        public static readonly int[] MODE_OPTIONS = new int[] { -1, 9, 99, 999, 9999, 99999 };
        public int mode_selected = 0;

        public GameSetup()
        {
            menuHandler.RegisterOption('S', () => { state = State.Finished; });
            menuHandler.RegisterParametricOption("JFK", ChangeAI);
            menuHandler.RegisterOption('P', () => { state = State.Rules; });
            menuHandler.RegisterOption('M', ChangeMode);

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
                    Renderer.PRINT.G().P("| ").H("Start | Změň AI ").H("Jardy, ").H("Franty nebo ").H("Karla | ").H("Pravidla | ").H("Mód: ");
                    if (MODE_OPTIONS[mode_selected] == -1)
                    {
                        Renderer.PRINT.P("normální |");
                    }
                    else
                    {
                        Renderer.PRINT.P($"simulace {MODE_OPTIONS[mode_selected]} her |");
                    }
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

        void ChangeMode()
        {
            mode_selected = (mode_selected + 1) % MODE_OPTIONS.Length;
        }

        public Game NewGame()
        {
            Renderer.PRINT.CLR().R();
            return new Game(players, MODE_OPTIONS[mode_selected]);
        }
    }
}