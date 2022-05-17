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
            public Func<PlayerController> GetNew;
            public int id;
            public Type(string label, Func<PlayerController> getNew, bool isHuman = false)
            {
                this.label = label;
                GetNew = getNew;
                this.id = TYPES.Count;
                TYPES.Add(this);
            }
        }
        public static readonly Type HUMAN = new Type(" Člověk ", () => new HumanPlayerController());
        public bool isHuman = false;
        public Player player;
        public virtual void GetOptions(Game g)
        {
            Renderer.PRINT.P($"{Utils.TranslatePlayer(g.activePlayer)} is thinking...");
            Utils.Wait(1000);
        }
        public abstract int ChooseTrumps();

        public abstract int ChooseTalon();
    }
}