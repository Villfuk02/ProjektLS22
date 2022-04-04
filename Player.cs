using System;

namespace ProjektLS22
{
    public class Player
    {
        public ConsoleColor color;
        PlayerController controller;
        public int cash = 50;
        public Pos basePos;

        public Player(ConsoleColor color, PlayerController controller)
        {
            this.color = color;
            this.controller = controller;
        }

        public Action GetIntent(World w)
        {
            return controller.GetIntent(w, this);
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