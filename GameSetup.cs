using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class GameSetup
    {
        public static readonly int PLAYER_AMT = 3;
        PlayerController.Type[] players = new PlayerController.Type[PLAYER_AMT];
        enum State { Menu, Rules, Finished };
        State state = State.Menu;
        public static readonly int[] CASH_OPTIONS = new int[] { 20, 50, 100, 200, 500, 1000 };
        public int cash_selected = 2;

        public GameSetup()
        {
            for (int i = 0; i < PLAYER_AMT; i++)
            {
                players[i] = PlayerController.HUMAN;
            }
            while (state != State.Finished)
            {
                PrintInfo();
                ProcessInput(Console.ReadKey(true));
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
                for (int i = 0; i < PLAYER_AMT; i++)
                {
                    Renderer.PRINT.W().P(" ").B(ConsoleColor.Blue).P(" Hráč ").P(i + 1, 1).P(" ").B().P("  ");
                }
                Renderer.PRINT.NL();
                for (int i = 0; i < PLAYER_AMT; i++)
                {
                    Renderer.PRINT.W().P(" ").B(ConsoleColor.Blue).P(players[i].label, 8, true).B().P("  ");
                }
            }
            Renderer.PRINT.NL().NL();
            switch (state)
            {
                case State.Menu:
                    Renderer.PRINT.G().P("| ").H("Start | Změň AI hráče ").H("1 až ").H(PLAYER_AMT.ToString()).P(" | ").H("Pravidla |");
                    break;
                case State.Rules:
                    Renderer.PRINT.G().P("| ").H("Start | ").H("Zpět do menu |");
                    break;
            }
            Renderer.PRINT.NL();
        }

        void ProcessInput(ConsoleKeyInfo k)
        {
            switch (state)
            {
                case State.Menu:
                    switch (k.Key)
                    {
                        case ConsoleKey.S:
                            state = State.Finished;
                            break;
                        case ConsoleKey.P:
                            state = State.Rules;
                            break;
                        default:
                            char c = k.KeyChar;
                            if (c >= '1' && c <= '0' + PLAYER_AMT)
                            {
                                ChangeAI(c - '1');
                            }
                            break;
                    }
                    break;
                case State.Rules:
                    switch (k.Key)
                    {
                        case ConsoleKey.S:
                            state = State.Finished;
                            break;
                        case ConsoleKey.Z:
                            state = State.Menu;
                            break;
                    }
                    break;
            }
        }

        void ChangeAI(int player)
        {
            int a = (players[player].id + 1) % PlayerController.TYPES.Count;
            players[player] = PlayerController.TYPES[a];
        }

        public Game NewGame()
        {
            return new Game(players);
        }
    }
}