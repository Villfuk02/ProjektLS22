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
        public static readonly Type RANDOM = new Type("RandomAI", () => new RandomAI());
        public static readonly Type NAIVE = new Type("NaiveAI", () => new NaiveAI());
        public static readonly Type SMART = new Type("SmartAI", () => new SmartAI());
        public bool isHuman = false;
        public Player player;
        public virtual void GetOptions(Game.Phase phase, int step, Card trumps, List<Card> trick)
        {
            Renderer.PRINT.P($"{Utils.TranslatePlayer(player.index)} přemýšlí...");
        }
        public virtual void NewRound(int dealer) { }
        public virtual void FirstTrickStart(Card trumps, bool fromPeople, List<Card> talonIfKnown) { }
        public virtual void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage) { }
        public virtual void TakesTrick(int p, List<Card> trick) { }
        public abstract int ChooseTrumps();
        public abstract int ChooseTalon(Card trumps, List<Card> talon);
        public abstract int ChoosePlay(List<Card> trick, Card trumps);
    }
}