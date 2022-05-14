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
            public bool isHuman;
            public Type(string label, Func<PlayerController> GetNew, bool isHuman = false)
            {
                this.label = label;
                this.GetNew = GetNew;
                this.id = TYPES.Count;
                this.isHuman = isHuman;
                TYPES.Add(this);
            }
        }
        public static readonly Type HUMAN = new Type(" Člověk", () => new HumanPlayerController(), true);
        public bool isHuman = false;
        public abstract void GetOptions(Game g);
    }
}