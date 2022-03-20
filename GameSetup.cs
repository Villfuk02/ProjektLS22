using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class GameSetup
    {
        static readonly ConsoleColor[] COLORS = new ConsoleColor[]{
                ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta, ConsoleColor.DarkCyan, ConsoleColor.DarkYellow};
        static readonly int WORLD_SIZE_MIN = 10;
        static readonly int WORLD_SIZE_MAX = 30;
        static readonly int PLAYER_AMOUNT_MIN = 2;
        static readonly int PLAYER_AMOUNT_MAX = 6;

        int worldSize = 16;
        List<PlayerData> players = new List<PlayerData>() {
            new PlayerData(ConsoleColor.DarkBlue, PlayerController.Type.Human),
            new PlayerData(ConsoleColor.DarkRed, PlayerController.Type.Human)
            };
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

        public World InitWorld()
        {
            Player[] p = new Player[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                p[i] = new Player(players[i].color, PlayerController.GetNew(players[i].type));
            }
            return new World(worldSize, p);
        }
    }
}