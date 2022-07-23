using System.Collections.Generic;
using static ProjektLS22.Utils;
namespace ProjektLS22
{
    public static class CombinationGenerator
    {
        public readonly struct PileTriple
        {
            readonly Pile p0;
            readonly Pile p1;
            readonly Pile p2;
            public Pile this[int i] => i == 0 ? p0 : (i == 1 ? p1 : p2);
            public PileTriple(Pile p0, Pile p1, Pile p2)
            {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
            }
            public PileTriple(PileTriple original, int pileToEdit, uint removeCardMask)
            {
                p0 = original[0];
                p1 = original[1];
                p2 = original[2];
                Pile e = original[pileToEdit];
                e.With(removeCardMask);
                switch (pileToEdit)
                {
                    case 0:
                        p0 = e;
                        break;
                    case 1:
                        p1 = e;
                        break;
                    case 2:
                        p2 = e;
                        break;
                }
            }
        }
        static List<PileTriple> GetRawCombinations(int count1, int count2, Pile talon, in PileTriple masks, in Pile available)
        {
            List<PileTriple> combinations = new List<PileTriple>();
            void Generate(PileTriple hands, in int[] maxCounts, in PileTriple masks, in Pile available, uint cardMask)
            {
                if (cardMask == 0)
                {
                    combinations.Add(hands);
                }
                else
                {
                    if (available.HasAny(cardMask))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (hands[i].Count < maxCounts[i] && masks[i].HasAny(cardMask))
                            {
                                Generate(new PileTriple(hands, i, cardMask), maxCounts, masks, available, cardMask << 1);
                            }
                        }
                    }
                    else
                    {
                        Generate(hands, maxCounts, masks, available, cardMask << 1);
                    }
                }
            }
            Generate(new PileTriple(new Pile(), new Pile(), talon), new int[] { count1, count2, 2 }, masks, available, 1);
            return combinations;
        }

        public static IEnumerable<Pile[]> AllCombinationsRandomOrder(SinglePlayerPrediction[] models, int fixedPlayer, Pile hand, Pile? talonIfKnown, int startingPlayer, Pile valid)
        {
            Pile t = new Pile();
            if (talonIfKnown.HasValue)
            {
                t = talonIfKnown.Value;
                valid = valid.Without(t);
            }
            List<int> maxCards = new List<int>();
            int remove = hand.Count * 2 - valid.Count + 3 + t.Count;
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
            List<Pile> maskList = new List<Pile>();
            for (int i = 0; i < 3; i++)
            {
                maskList.Add((Pile)models[i]);
            }
            maskList.RemoveAt(fixedPlayer);
            PileTriple masks = new PileTriple(maskList[0], maskList[1], Value.GivesPointsMask);
            List<PileTriple> combinations = GetRawCombinations(maxCards[0], maxCards[1], t, masks, valid);
            for (int i = 0; i < combinations.Count; i++)
            {
                int r = _rand.Next(i, combinations.Count);
                var p = combinations[r];
                combinations[r] = combinations[i];
                yield return Translate(p, fixedPlayer, hand);
            }
        }

        static Pile[] Translate(PileTriple combination, int fixedPlayer, Pile hand)
        {
            List<Pile> res = new List<Pile>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(combination[i]);
            }
            res.Insert(fixedPlayer, hand);
            return res.ToArray();
        }
    }
}