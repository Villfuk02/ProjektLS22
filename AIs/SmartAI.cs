using System;
using System.Collections.Generic;
using System.Linq;
using static ProjektLS22.Utils;


namespace ProjektLS22
{
    public class SmartAI : GameStateKeepingAI
    {
        public override int ChooseTrumps()
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>(from s in Suit.ALL select new KeyValuePair<Suit, int>(s, 0));
            for (int i = 0; i < 7; i++)
            {
                Card c = player.hand[i];
                strengths[c.suit] += c.value.strength;
            }
            KeyValuePair<Suit, int> max = new KeyValuePair<Suit, int>(null, 0);
            int average = 0;
            foreach (KeyValuePair<Suit, int> p in strengths)
            {
                if (p.Value > max.Value)
                    max = p;
                average += p.Value;
            }
            average /= 4;
            if (max.Value < 37 || max.Value - average < 10)
            {
                return -1;
            }
            for (int i = 0; i < 7; i++)
            {
                if (max.Key.HasCard(player.hand[i]))
                    return i;
            }
            //WILL NEVER HAPPEN
            return -2;
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>(from s in Suit.ALL select new KeyValuePair<Suit, int>(s, 0));
            Dictionary<Suit, bool> married = new Dictionary<Suit, bool>(from s in Suit.ALL select new KeyValuePair<Suit, bool>(s, player.hand.Count(c => s.HasCard(c) && c.value.marriage) == 2));
            for (int i = 0; i < player.hand.Count; i++)
            {
                Card c = player.hand[i];
                strengths[c.suit] += c.value.strength;
            }
            strengths[trumps.suit] = 999;
            IEnumerable<(Card, int)> sortedCards = player.hand.Select((c, i) => (c, i))
                                                              .Where(v => _ValidTalon(player.hand, v.i, trumps, null))
                                                              .OrderBy(v => strengths[v.c.suit])
                                                              .ThenBy(v => v.c.value.strength);
            bool IsNotMarried((Card, int) v)
            {
                return !(v.Item1.value.marriage && married[v.Item1.suit]);
            }
            if (sortedCards.Any(IsNotMarried))
                return sortedCards.First(IsNotMarried).Item2;
            else
                return sortedCards.First().Item2;
        }
        public override int ChoosePlay(List<Card> trick, Card trumps)
        {
            int[] ratings = new int[player.hand.Count];
            Func<Card, List<Card>, Suit, int> rateOption = null;
            switch (trick.Count)
            {
                case 0:
                    {
                        if (isOffense)
                            rateOption = RateFirstCardOffense;
                        else if (isFirstDefense)
                            rateOption = RateFirstCardFirstDefense;
                        else
                            rateOption = RateFirstCardSecondDefense;
                        break;
                    }
                case 1:
                    {
                        rateOption = RateSecondCard;
                        break;
                    }
                case 2:
                    {
                        rateOption = RateLastCard;
                        break;
                    }
            }

            for (int i = 0; i < player.hand.Count; i++)
            {
                Card c = player.hand[i];
                //discourage playing high cards
                ratings[i] -= c.value.strength;
                if (_ValidPlay(player.hand, i, trumps, trick))
                {
                    ratings[i] += rateOption(c, trick, trumps.suit);
                }
                else
                {
                    //block invalid cards
                    ratings[i] = -1_000_000_000;
                }
            }
            return ratings.Select((n, i) => (n, i)).Max().i;
        }

        int RateFirstCardOffense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            //X - A risk -1M
            if (c.value == Value.DESET)
            {
                if (others.Contains(new Card(c.suit, Value.ESO)))
                {
                    rating -= 1_000_000;
                }
            }
            //A/X - trump risk -500K*expected
            float[] trumpedChances = new float[3];
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.suit);
            if (c.suit != trumps && others.Any(trumps.HasCard))
            {
                OtherPlayersSuitInfo trumpInfo = GetOtherPlayersSuitInfo(trumps);
                for (int j = 1; j <= 2; j++)
                {
                    trumpedChances[j] = suitInfo.noneChance[j] * (1 - trumpInfo.noneChance[j]);
                }
            }
            if (c.value.ten)
            {
                rating -= (int)(500_000 * (trumpedChances[1] + trumpedChances[2]));
            }
            //A - X force take +20K*chance
            if (c.value == Value.ESO)
            {
                if (others.Contains(new Card(c.suit, Value.DESET)))
                {
                    float expected = 0;
                    for (int j = 1; j <= 2; j++)
                    {
                        if (!players[_PPlus(player.index, j)].Contains(new Card(c.suit, Value.DESET)))
                            continue;
                        float tenChance = 0;
                        if (!players[_PPlus(player.index, 3 - j)].Contains(new Card(c.suit, Value.DESET)))
                        {
                            if (suitInfo.max[j] == 1)
                            {
                                tenChance = 1;
                            }
                            else if (suitInfo.min[j] >= 2)
                            {
                                tenChance = 0;
                            }
                            else
                            {
                                tenChance = OneHalfPower(suitInfo.max[j] - 1);
                            }
                        }
                        else
                        {
                            tenChance = OneHalfPower(suitInfo.max[j]);
                        }
                        expected += tenChance * (1 - trumpedChances[3 - j]);
                    }
                    rating += (int)(20_000 * expected);
                }
            }
            //trump - -5K
            if (trumps.HasCard(c))
                rating -= 5_000;
            //any - trump pull +1K*expected
            if (trumps.HasCard(c))
            {
                rating += (int)(1_000 * (2 - suitInfo.noneChance[1] - suitInfo.noneChance[2]));
            }
            else
            {
                rating += (int)(1_000 * (trumpedChances[1] + trumpedChances[2]));
            }

            return rating;
        }
        int RateFirstCardFirstDefense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            //-10K if enemy could steal X or A
            if (c.value.ten)
            {
                if (prevPlayer.Contains(new Card(c.suit, Value.ESO))
                    || (c.suit != trumps && nextPlayer.Any(trumps.HasCard)
                    && nextPlayer.Count(c.SameSuit) == others.Count(c.SameSuit)))
                {
                    rating -= 10_000;
                }
            }
            //relative harm
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.suit);
            rating += (int)(1000 * (suitInfo.noneChance[2] - suitInfo.noneChance[1]));
            //trump - -5K
            if (trumps.HasCard(c))
                rating -= 5_000;
            return rating;
        }

        int RateFirstCardSecondDefense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.suit);
            OtherPlayersSuitInfo trumpInfo = GetOtherPlayersSuitInfo(trumps);
            HashSet<Card> guaranteed = new HashSet<Card>(others);
            guaranteed.ExceptWith(prevPlayer);
            HashSet<Card> possible = new HashSet<Card>(nextPlayer);
            possible.ExceptWith(guaranteed);
            //-10K if enemy could steal X or A
            if (c.value.ten)
            {
                if (possible.Contains(new Card(c.suit, Value.ESO)) || (c.suit != trumps && trumpInfo.max[1] > 0 && trumpInfo.min[1] <= 0))
                {
                    rating -= 10_000;
                }
            }
            //+10K * chance to steal X or A
            if (c.suit != trumps)
            {
                if (!guaranteed.Any(d => c.SameSuitButLowerThan(d) && !d.value.ten))
                {
                    float stealChance = 0;
                    float noLowerChance = OneHalfPower(possible.Count(d => c.SameSuitButLowerThan(d) && !d.value.ten));
                    if (guaranteed.Contains(new Card(c.suit, Value.DESET)) || guaranteed.Contains(new Card(c.suit, Value.ESO)))
                    {
                        float takeChance = 0;
                        if (possible.Contains(new Card(c.suit, Value.ESO)))
                        {
                            takeChance += 0.5f;
                        }
                        takeChance += suitInfo.noneChance[2] * (1 - trumpInfo.noneChance[2]);
                        stealChance += noLowerChance * takeChance;
                    }
                    else
                    {
                        if (possible.Contains(new Card(c.suit, Value.DESET)))
                        {
                            float takeChance = 0;
                            if (possible.Contains(new Card(c.suit, Value.ESO)))
                            {
                                takeChance += 0.5f;
                            }
                            takeChance += suitInfo.noneChance[2] * (1 - trumpInfo.noneChance[2]);
                            stealChance += noLowerChance * takeChance * 0.5f;
                        }
                        if (possible.Contains(new Card(c.suit, Value.ESO)))
                        {
                            float takeChance = 0;
                            takeChance += suitInfo.noneChance[2] * (1 - trumpInfo.noneChance[2]);
                            stealChance += noLowerChance * takeChance * 0.5f;
                        }
                    }
                    rating += (int)(10_000 * stealChance);
                }
            }

            //1K * relative harm
            rating += (int)(1000 * (suitInfo.noneChance[1] - suitInfo.noneChance[2]));

            //trump - -5K
            if (trumps.HasCard(c))
                rating -= 5_000;
            return rating;
        }
        int RateSecondCard(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;

            Card first = trick[0];
            List<Card> updatedTrick = new List<Card> { first, c };
            //100 * excpected points * (win chance - loss chance)
            int pts = 10 * updatedTrick.Count(d => d.value.ten);
            int bestSoFar = _TrickWinner(updatedTrick, trumps);
            Card best = updatedTrick[bestSoFar];

            float winChance;
            if (IsTrickIndexEnemyWhenImSecond(bestSoFar) == IsTrickIndexEnemyWhenImSecond(2))
            {
                winChance = IsTrickIndexEnemyWhenImSecond(bestSoFar) ? 0 : 1;
            }
            else
            {
                HashSet<Card> guaranteed = new HashSet<Card>(others);
                guaranteed.ExceptWith(prevPlayer);
                HashSet<Card> possible = new HashSet<Card>(nextPlayer);
                possible.ExceptWith(guaranteed);
                float beatsBestChance;
                OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(first.suit);
                bool WillWinTrick(Card d)
                {
                    return _TrickWinner(new List<Card> { first, c, d }, trumps) == 2;
                }
                int winningCards = possible.Count(d => first.SameSuit(d) && WillWinTrick(d));
                if (suitInfo.min[1] > 0)
                {
                    if (guaranteed.Any(d => first.SameSuit(d) && WillWinTrick(d)))
                    {
                        beatsBestChance = 1;
                    }
                    else
                    {

                        beatsBestChance = OneHalfPower(winningCards);
                    }
                }
                else
                {
                    float beatsBestWithSuit;
                    if (suitInfo.max[2] > 0)
                    {
                        beatsBestWithSuit = 1 - ((1 << (suitInfo.max[2] - winningCards)) - 1) / (float)((1 << suitInfo.max[2]) - 1);
                    }
                    else
                    {
                        beatsBestWithSuit = 0;
                    }
                    if (guaranteed.Any(d => trumps.HasCard(d) && WillWinTrick(d)))
                    {
                        beatsBestChance = suitInfo.noneChance[2] + (1 - suitInfo.noneChance[2]) * beatsBestWithSuit;
                    }
                    else
                    {
                        int winningTrumpCards = possible.Count(d => trumps.HasCard(d) && _TrickWinner(new List<Card> { first, c, d }, trumps) != 1);
                        beatsBestChance = suitInfo.noneChance[2] * (1 - OneHalfPower(winningTrumpCards)) + (1 - suitInfo.noneChance[2]) * beatsBestWithSuit;
                    }
                }
                winChance = 0;
                if (!IsTrickIndexEnemyWhenImSecond(2))
                {
                    winChance += beatsBestChance;
                }
                if (!IsTrickIndexEnemyWhenImSecond(bestSoFar))
                {
                    winChance += 1 - beatsBestChance;
                }
            }
            rating += (int)(100 * (2 * winChance - 1) * (pts + 1));

            //penalty for playing A when X is not secured
            if (c.value == Value.ESO && others.Contains(new Card(c.suit, Value.DESET)))
            {
                rating -= 1000;
            }

            return rating;
        }

        int RateLastCard(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            List<Card> updatedTrick = new List<Card>(trick);
            updatedTrick.Add(c);
            int winner = _TrickWinner(updatedTrick, trumps);
            if (winner == 2 || (!isOffense && (winner == (isFirstDefense ? 0 : 1))))
            {
                if (c.value == Value.DESET && others.Contains(new Card(c.suit, Value.ESO)))
                {
                    rating += 100_000;
                }
                else if (c.value.ten && c.suit != trumps)
                {
                    rating += 10_000 * (others.Count(trumps.HasCard) - us.Count(trumps.HasCard));
                }
            }
            else if (c.value.ten)
            {
                rating -= 1_000_000;
            }
            if (me.Any(d => c.SameSuit(d) && d.value.ten))
            {
                rating += 100 * (me.Count(c.SameSuit)) - 1_000;
            }
            else
            {
                rating += 1_000 - 100 * (me.Count(c.SameSuit));
            }
            return rating;
        }

        OtherPlayersSuitInfo GetOtherPlayersSuitInfo(Suit s)
        {
            int totalCards = others.Count(s.HasCard);
            int[] maxCards = new int[3];
            for (int p = 1; p <= 2; p++)
            {
                maxCards[p] = players[_PPlus(player.index, p)].Count(s.HasCard);
            }
            return new OtherPlayersSuitInfo(totalCards, maxCards);
        }

        bool IsTrickIndexEnemyWhenImSecond(int i)
        {
            switch (i)
            {
                case 0:
                    return !isSecondDefense;
                case 2:
                    return !isFirstDefense;
                default:
                    return false;
            }
        }

        struct OtherPlayersSuitInfo
        {
            public int total;
            public int[] max;
            public int[] min;
            public float[] noneChance;
            public OtherPlayersSuitInfo(int total, int[] max)
            {
                this.total = total;
                this.max = max;
                min = new int[3];
                noneChance = new float[3];
                for (int p = 1; p <= 2; p++)
                {
                    min[p] = total - max[3 - p];
                    if (min[p] <= 0)
                    {
                        noneChance[p] = OneHalfPower(max[p]);
                    }
                }
            }
        }
        static float OneHalfPower(int pow)
        {
            return 1f / (1 << pow);
        }
    }
}
