using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public class PlayerController
    {
        public static List<Type> TYPES = new List<Type>();
        public struct Type
        {
            public string label;
            public Func<PlayerController> GetNew;
            public int id;
            public Type(string label, Func<PlayerController> GetNew)
            {
                this.label = label;
                this.GetNew = GetNew;
                this.id = TYPES.Count;
                TYPES.Add(this);
            }
        }
        public static readonly Type HUMAN = new Type("Human", () => new HumanPlayerController());
    }
}