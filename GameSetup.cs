using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class GameSetup
    {
        static readonly ConsoleColor[] COLORS = new ConsoleColor[]{
                ConsoleColor.DarkBlue, ConsoleColor.DarkRed, ConsoleColor.DarkGreen,
                ConsoleColor.DarkMagenta, ConsoleColor.DarkCyan, ConsoleColor.DarkYellow};
        static readonly string COLOR_CODES = "BRGMCY";
        static readonly int WORLD_SIZE_MIN = 9;
        static readonly int WORLD_SIZE_MAX = 30;
        static readonly int PLAYER_AMOUNT_MIN = 2;
        static readonly int PLAYER_AMOUNT_MAX = 6;

        int worldSize = 12;
        List<PlayerData> players = new List<PlayerData>() { };
        struct PlayerData
        {
            public ConsoleColor color;
            public PlayerController.Type type;
            public PlayerData(ConsoleColor color, PlayerController.Type type)
            {
                this.color = color;
                this.type = type;
            }
        }

        enum State { Menu, World, Player, Finished };
        State state = State.Menu;
        int selectedPlayer = -1;
        bool changedWorldSize = false;

        int[] colorUsers = new int[COLORS.Length];

        public GameSetup()
        {
            for (int i = 0; i < colorUsers.Length; i++)
            {
                colorUsers[i] = -1;
            }
            while (state != State.Finished)
            {
                while (players.Count < PLAYER_AMOUNT_MIN)
                {
                    AddPlayer();
                }
                if (!changedWorldSize)
                    worldSize = RecommendedSize();
                PrintInfo();
                ProcessInput(Console.ReadKey(true));
            }
        }

        void PrintInfo()
        {
            Renderer.PRINT.C().R().G().P("Current game settings:").NL();
            Renderer.PRINT.P("World size: ").W().P(worldSize, 2).S(5).G().P("Players: ").W().P(players.Count, 1).NL();
            for (int i = 0; i < players.Count; i++)
            {
                if (selectedPlayer == i)
                    Renderer.PRINT.W().P(">").B(players[i].color).P("Player ").P(i + 1, 1).B().P("< ");
                else
                    Renderer.PRINT.G().P(" ").B(players[i].color).P("Player ").P(i + 1, 1).B().P("  ");
            }
            Renderer.PRINT.NL();
            for (int i = 0; i < players.Count; i++)
            {
                Renderer.PRINT.G().P(" ").B(players[i].color).P(players[i].type.label, 8, true).B().P("  ");
            }
            Renderer.PRINT.NL().NL();
            switch (state)
            {
                case State.Menu:
                    Renderer.PRINT.G().P("| ").H("Start | ").H("World size | Edit player ").H("1 to ").H(players.Count.ToString()).P(" |");
                    if (players.Count < PLAYER_AMOUNT_MAX)
                        Renderer.PRINT.P(" ").H("Add player |");
                    break;
                case State.World:
                    Renderer.PRINT.G().P("| ").H("Start | ").H("Finish editing world |").H("Decrease size | ").H("Grow size | ").H("Recommended size (").P(RecommendedSize(), 2).P(") |");
                    break;
                case State.Player:
                    Renderer.PRINT.G().P("| ").H("Start | ").H("Finish editing player | Select color ").F(ConsoleColor.Black);
                    for (int i = 0; i < COLORS.Length; i++)
                    {
                        if (COLORS[i] != players[selectedPlayer].color)
                            Renderer.PRINT.B(COLORS[i]).P(COLOR_CODES[i]).S(1);
                    }
                    Renderer.PRINT.R().G().P(" | Change ").H("AI | ").H("Delete player |");
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
                        case ConsoleKey.W:
                            state = State.World;
                            break;
                        case ConsoleKey.A:
                            if (players.Count < PLAYER_AMOUNT_MAX)
                                AddPlayer();
                            break;
                        default:
                            char c = k.KeyChar;
                            if (c >= '1' && c <= '0' + players.Count)
                            {
                                selectedPlayer = c - '1';
                                state = State.Player;
                            }
                            break;
                    }
                    break;
                case State.World:
                    switch (k.Key)
                    {
                        case ConsoleKey.S:
                            state = State.Finished;
                            break;
                        case ConsoleKey.F:
                            state = State.Menu;
                            break;
                        case ConsoleKey.D:
                            worldSize -= 3;
                            if (worldSize < WORLD_SIZE_MIN)
                                worldSize = WORLD_SIZE_MIN;
                            changedWorldSize = true;
                            break;
                        case ConsoleKey.G:
                            worldSize += 3;
                            if (worldSize > WORLD_SIZE_MAX)
                                worldSize = WORLD_SIZE_MAX;
                            changedWorldSize = true;
                            break;
                        case ConsoleKey.R:
                            worldSize = RecommendedSize();
                            changedWorldSize = true;
                            break;
                    }
                    break;
                case State.Player:
                    switch (k.Key)
                    {
                        case ConsoleKey.S:
                            state = State.Finished;
                            break;
                        case ConsoleKey.F:
                            state = State.Menu;
                            selectedPlayer = -1;
                            break;
                        case ConsoleKey.A:
                            ChangeAI();
                            break;
                        case ConsoleKey.D:
                            DeletePlayer();
                            state = State.Menu;
                            selectedPlayer = -1;
                            break;
                        default:
                            char c = k.KeyChar;
                            c = Char.ToUpper(c);
                            for (int i = 0; i < COLORS.Length; i++)
                            {
                                if (COLORS[i] != players[selectedPlayer].color && c == COLOR_CODES[i])
                                    ChangeColor(i);
                            }
                            break;
                    }
                    break;
            }
        }

        void AddPlayer()
        {
            ConsoleColor c = ConsoleColor.Black;
            for (int i = 0; i < COLORS.Length; i++)
            {
                if (colorUsers[i] == -1)
                {
                    c = COLORS[i];
                    colorUsers[i] = players.Count;
                    break;
                }
            }
            players.Add(new PlayerData(c, PlayerController.HUMAN));
        }

        void DeletePlayer()
        {

            for (int i = 0; i < COLORS.Length; i++)
            {
                if (colorUsers[i] == selectedPlayer)
                {
                    colorUsers[i] = -1;
                }
                else if (colorUsers[i] > selectedPlayer)
                {
                    colorUsers[i]--;
                }
            }
            players.RemoveAt(selectedPlayer);
        }

        void ChangeAI()
        {
            PlayerData p = players[selectedPlayer];
            int a = (p.type.id + 1) % PlayerController.TYPES.Count;
            p.type = PlayerController.TYPES[a];
            players[selectedPlayer] = p;
        }
        void ChangeColor(int new_color)
        {
            int c = colorUsers[new_color];
            if (c != -1)
            {
                PlayerData prev_user = players[c];
                prev_user.color = players[selectedPlayer].color;
                players[c] = prev_user;
            }
            PlayerData new_user = players[selectedPlayer];
            new_user.color = COLORS[new_color];
            players[selectedPlayer] = new_user;
            for (int i = 0; i < COLORS.Length; i++)
            {
                if (colorUsers[i] == selectedPlayer)
                {
                    colorUsers[i] = c;
                    break;
                }
            }
            colorUsers[new_color] = selectedPlayer;
        }

        int RecommendedSize()
        {
            return 6 + 3 * players.Count;
        }

        public World InitWorld()
        {
            Player[] p = new Player[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                p[i] = new Player(players[i].color, players[i].type.GetNew());
            }
            return new World(worldSize, p);
        }
    }
}