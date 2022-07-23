using System;
using System.Collections.Generic;
using System.Security;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    public abstract class PlayerController
    {
        public static readonly List<Type> TYPES = new List<Type>();
        public struct Type
        {
            public readonly string label;
            public readonly Func<Player, PlayerController> GetNew;
            public readonly int id;
            public Type(string label, Func<Player, PlayerController> getNew)
            {
                this.label = label;
                GetNew = getNew;
                this.id = TYPES.Count;
                TYPES.Add(this);
            }
        }
        public static readonly Type HUMAN = new Type(" Člověk ", (p) => new HumanPlayerController(p));
        public static readonly Type RANDOM = new Type("RandomAI", (p) => new RandomAI(p));
        public static readonly Type SMART = new Type("SmartAI", (p) => new SmartAI(p));
        public static readonly Type HYBRID = new Type("HybridAI", (p) => new SimulationAI(p, 7, int.MaxValue));
        public static readonly Type SIM_10K = new Type("Sim-10K", (p) => new SimulationAI(p, 9, 10_000));
        public static readonly Type SIM_100K = new Type("Sim-100K", (p) => new SimulationAI(p, 9, 100_000));
        public static readonly Type SIM_1M = new Type("Sim-1M", (p) => new SimulationAI(p, 9, 1000_000));
        public static readonly Type SIM_FULL = new Type("Sim-Full", (p) => new SimulationAI(p, 10, int.MaxValue));
        bool isHuman;
        Player player;
        public PlayerController(Player p, bool isHuman)
        {
            this.isHuman = isHuman;
            player = p;
        }
        public virtual void GetOptions(Game.Phase phase, int step, Card? trumps, List<Card> trick)
        {
            _printer.P($"{_playerNames[player.index]} přemýšlí...");
        }
        public virtual void NewRound(int dealer) { }
        public virtual void FirstTrickStart(Card trumps, bool fromPeople, int offense, Pile? talonIfKnown) { }
        public virtual void PlaysCard(int p, Card c, List<Card> trick, Card trumps, bool marriage) { }
        public virtual void TakesTrick(int p, List<Card> trick) { }
        /// <summary>
        /// Choose trumps. Return null to choose from the people.
        /// </summary>
        public abstract Card? ChooseTrumps();
        public abstract Card ChooseTalon(Card trumps, Pile talon);
        public abstract Card ChoosePlay(List<Card> trick, Card trumps);
        protected Pile Hand { get => new Pile(player.hand); }
        protected int Index { get => player.index; }
        public bool IsHuman { get => isHuman; }
    }
}