using System;

namespace ProjektLS22
{
    public class Player
    {
        public ConsoleColor color;
        PlayerController controller;
        public int energy = 50;

        public Player(ConsoleColor color, PlayerController controller)
        {
            this.color = color;
            this.controller = controller;
        }

        public Action GetIntent(World w)
        {
            return controller.GetIntent(w);
        }

        public struct Action
        {
            char item;
            Pos pos;
            public Action(char item, Pos pos)
            {
                this.item = item;
                this.pos = pos;
            }
        }
    }
}