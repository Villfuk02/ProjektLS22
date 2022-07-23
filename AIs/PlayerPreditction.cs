using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

namespace ProjektLS22
{
    public abstract class PlayerPrediction
    {
        public abstract Pile ToPile();
        public abstract void Remove(Card c);
        public abstract void Remove(Pile p);
        public abstract PlayerPrediction Copy();
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
        public override PlayerPrediction Copy()
        {
            return new SinglePlayerPrediction(this);
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
        public override PlayerPrediction Copy()
        {
            return new JointPlayerPrediction(models.Select(m => m.Copy()).ToArray());
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