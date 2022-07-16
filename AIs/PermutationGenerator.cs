using System;
using System.Collections.Generic;
using System.Threading.Channels;
using static ProjektLS22.Utils;
namespace ProjektLS22
{
    public static class PermutationGenerator
    {
        struct Pile
        {
            public uint current;
            public int count;
            public Pile(int count)
            {
                current = 0;
                this.count = count;
            }
            public Pile(Pile p, uint c)
            {
                current = p.current | c;
                count = p.count - 1;
            }
        }
        static List<(uint, uint, uint)> GetRawPermutations(int count1, int count2, Pile talon, in (uint, uint, uint) masks, in uint available)
        {
            List<(uint, uint, uint)> permutations = new List<(uint, uint, uint)>();
            void Generate(Pile p1, Pile p2, Pile p3, in (uint, uint, uint) masks, in uint available, uint card)
            {
                if (card == 0)
                {
                    permutations.Add((p1.current, p2.current, p3.current));
                }
                else
                {
                    if ((available & card) != 0)
                    {
                        if (p1.count > 0 && (masks.Item1 & card) != 0)
                        {
                            Generate(new Pile(p1, card), p2, p3, masks, available, card << 1);
                        }
                        if (p2.count > 0 && (masks.Item2 & card) != 0)
                        {
                            Generate(p1, new Pile(p2, card), p3, masks, available, card << 1);
                        }
                        if (p3.count > 0 && (masks.Item3 & card) != 0)
                        {
                            Generate(p1, p2, new Pile(p3, card), masks, available, card << 1);
                        }
                    }
                    else
                    {
                        Generate(p1, p2, p3, masks, available, card << 1);
                    }
                }
            }
            Generate(new Pile(count1), new Pile(count2), talon, masks, available, 1);
            return permutations;
        }

        public static IEnumerable<List<Card>[]> AllPermutationsRandomOrder(SpecificPlayerModel[] models, int fixedPlayer, List<Card> hand, List<Card> talonIfKnown, int startingPlayer, IEnumerable<Card> valid)
        {
            Pile t = new Pile(2);
            List<Card> validCards = new List<Card>(valid);
            if (talonIfKnown != null)
            {
                foreach (Card c in talonIfKnown)
                {
                    t = new Pile(t, ((uint)1) << c.GetNum());
                    validCards.Remove(c);
                }
            }
            List<int> maxCards = new List<int>();
            int remove = hand.Count * 2 - validCards.Count + 3 + t.count;
            for (int i = 0; i < 3; i++)
            {
                maxCards.Add(hand.Count + 1);
            }
            int offset = startingPlayer;
            while (remove > 0)
            {
                maxCards[offset]--;
                remove--;
                offset = _PPlus(offset, 1);
            }
            maxCards.RemoveAt(fixedPlayer);
            List<uint> masks = new List<uint>();
            for (int i = 0; i < 3; i++)
            {
                masks.Add(Card.GetMask(models[i]));
            }
            masks.RemoveAt(fixedPlayer);
            List<(uint, uint, uint)> permutations = GetRawPermutations(maxCards[0], maxCards[1], t, (masks[0], masks[1], 0x77777777), Card.GetMask(validCards));
            for (int i = 0; i < permutations.Count; i++)
            {
                int r = _rand.Next(i, permutations.Count);
                var p = permutations[r];
                permutations[r] = permutations[i];
                yield return Translate(p, fixedPlayer, hand);
            }
        }

        static List<Card>[] Translate((uint, uint, uint) permutation, int fixedPlayer, List<Card> hand)
        {
            List<List<Card>> res = new List<List<Card>>();
            res.Add(Card.GetCards(permutation.Item1));
            res.Add(Card.GetCards(permutation.Item2));
            res.Add(Card.GetCards(permutation.Item3));
            res.Insert(fixedPlayer, new List<Card>(hand));
            return res.ToArray();
        }
    }
}