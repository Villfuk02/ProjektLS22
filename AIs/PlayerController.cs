using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public abstract class PlayerController
    {
        public static List<Type> TYPES = new List<Type>();
        public struct Type
        {
            public string label;
            public Func<Player, PlayerController> GetNew;
            public int id;
            public Type(string label, Func<Player, PlayerController> GetNew, bool isHuman = false)
            {
                this.label = label;
                this.GetNew = GetNew;
                this.id = TYPES.Count;
                TYPES.Add(this);
            }
        }
        public static readonly Type HUMAN = new Type(" Člověk ", (Player p) => new HumanPlayerController(p));
        public bool isHuman = false;
        Player player;
        public PlayerController(Player p)
        {
            player = p;
        }
        public virtual void GetOptions(Game g)
        {
            Renderer.PRINT.P($"{Utils.TranslatePlayer(g.activePlayer)} is thinking...");
            Utils.Wait(1000);
        }
        public abstract int ChooseTrumps();
    }
}