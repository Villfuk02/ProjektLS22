using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    public abstract class PlayerModel
    {
        public abstract HashSet<Card> MayHave();
    }

    public class SpecificPlayerModel : PlayerModel
    {
        HashSet<Card> mayHave = new HashSet<Card>();

        public void Reset()
        {
            mayHave.Clear();
            foreach (Suit s in Suit.ALL)
            {
                foreach (Value v in Value.ALL)
                {
                    mayHave.Add(new Card(s, v));
                }
            }
        }
        public void Remove(Card card)
        {
            mayHave.Remove(card);
        }
        public void RemoveRange(IEnumerable<Card> card)
        {
            mayHave.ExceptWith(card);
        }
        public void RemoveMatching(Predicate<Card> condition)
        {
            mayHave.RemoveWhere(condition);
        }
        public override HashSet<Card> MayHave()
        {
            return mayHave;
        }
    }

    public class JointPlayerModel : PlayerModel
    {
        PlayerModel[] models;
        public JointPlayerModel(params PlayerModel[] m)
        {
            models = m;
        }

        public override HashSet<Card> MayHave()
        {
            HashSet<Card> mayHave = new HashSet<Card>();
            foreach (var model in models)
            {
                mayHave.UnionWith(model.MayHave());
            }
            return mayHave;
        }
    }
}