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
    public abstract class PlayerModel : IEnumerable<Card>
    {
        protected abstract HashSet<Card> MayHave();
        public abstract void Remove(Card c);
        public abstract void RemoveRange(IEnumerable<Card> cards);
        public abstract void RemoveMatching(Predicate<Card> condition);
        public abstract PlayerModel Copy();
        public abstract void Reset();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<Card> GetEnumerator()
        {
            return MayHave().GetEnumerator();
        }
        public static implicit operator HashSet<Card>(PlayerModel p) => p.MayHave();
    }


    public class SpecificPlayerModel : PlayerModel
    {
        HashSet<Card> set = new HashSet<Card>();
        public SpecificPlayerModel() { }
        SpecificPlayerModel(HashSet<Card> cards)
        {
            set = new HashSet<Card>(cards);
        }

        public override void Reset()
        {
            set = new HashSet<Card>(Card.ALL);
        }
        public void Set(IEnumerable<Card> cards)
        {
            set = new HashSet<Card>(cards);
        }
        public override PlayerModel Copy()
        {
            return new SpecificPlayerModel(this);
        }
        public override void Remove(Card card)
        {
            set.Remove(card);
        }
        public override void RemoveRange(IEnumerable<Card> cards)
        {
            set.ExceptWith(cards);
        }
        public override void RemoveMatching(Predicate<Card> condition)
        {
            set.RemoveWhere(condition);
        }
        protected override HashSet<Card> MayHave()
        {
            return set;
        }
    }

    public class JointPlayerModel : PlayerModel
    {
        PlayerModel[] models;
        public JointPlayerModel(params PlayerModel[] m)
        {
            models = m.Where(m => m != null).ToArray();
        }

        public override void Reset()
        {
            foreach (PlayerModel m in models)
            {
                m.Reset();
            }
        }
        public override PlayerModel Copy()
        {
            return new JointPlayerModel(models.Select(m => m.Copy()).ToArray());
        }
        public override void Remove(Card card)
        {
            foreach (PlayerModel m in models)
            {
                m.Remove(card);
            }
        }
        public override void RemoveRange(IEnumerable<Card> cards)
        {
            foreach (PlayerModel m in models)
            {
                m.RemoveRange(cards);
            }
        }
        public override void RemoveMatching(Predicate<Card> condition)
        {
            foreach (PlayerModel m in models)
            {
                m.RemoveMatching(condition);
            }
        }
        protected override HashSet<Card> MayHave()
        {
            return new HashSet<Card>(models.Select(m => (IEnumerable<Card>)m).Aggregate((m, n) => m.Union(n)));
        }
    }
}