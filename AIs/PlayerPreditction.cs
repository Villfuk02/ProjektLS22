using System.Linq;

namespace ProjektLS22
{
    /// <summary>
    /// Stores the information a player knows about other players.
    /// </summary>
    public abstract class PlayerPrediction
    {
        public abstract Pile ToPile();
        public abstract void Remove(Card c);
        public abstract void Remove(Pile p);
        public abstract void Reset();
        public static implicit operator Pile(PlayerPrediction p) => p.ToPile();
    }


    public class SinglePlayerPrediction : PlayerPrediction
    {
        Pile mayHave = new Pile();
        public SinglePlayerPrediction() { }
        SinglePlayerPrediction(Pile cards)
        {
            Set(cards);
        }

        public override void Reset()
        {
            mayHave = Pile.FULL;
        }
        public void Set(Pile cards)
        {
            mayHave = cards;
        }
        public override void Remove(Card card)
        {
            mayHave = mayHave.Without(card);
        }
        public override void Remove(Pile pile)
        {
            mayHave = mayHave.Without(pile);
        }
        public override Pile ToPile()
        {
            return mayHave;
        }
    }

    public class JointPlayerPrediction : PlayerPrediction
    {
        PlayerPrediction[] models;
        public JointPlayerPrediction(params PlayerPrediction[] m)
        {
            models = m.Where(m => m != null).ToArray();
        }

        public override void Reset()
        {
            foreach (PlayerPrediction m in models)
            {
                m.Reset();
            }
        }
        public override void Remove(Card card)
        {
            foreach (PlayerPrediction m in models)
            {
                m.Remove(card);
            }
        }
        public override void Remove(Pile pile)
        {
            foreach (PlayerPrediction m in models)
            {
                m.Remove(pile);
            }
        }
        public override Pile ToPile()
        {
            Pile p = new Pile();
            foreach (PlayerPrediction m in models)
            {
                p.With(m);
            }
            return p;
        }
    }
}