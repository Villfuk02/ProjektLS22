using System;
using System.Collections.Generic;
using System.Linq;
using static ProjektLS22.Utils;


namespace ProjektLS22
{
    public class SmartAI : GameStateKeepingAI
    {
        public SmartAI(Player p) : base(p) { }

        public override Card? ChooseTrumps()
        {
            int[] strengths = new int[4];
            foreach (Card c in Hand.Enumerate())
            {
                strengths[c.Suit] += c.Value.Strength;
            }
            Suit? maxSuit = null;
            int maxStrength = 0;
            int average = 0;
            for (int i = 0; i < 4; i++)
            {
                if (strengths[i] > maxStrength)
                {
                    maxStrength = strengths[i];
                    maxSuit = new Suit(i);
                }
                average += strengths[i];
            }
            average /= 4;
            if (maxStrength < 37 || maxStrength - average < 10)
            {
                return null;
            }
            foreach (Card c in Hand.Enumerate())
            {
                if (maxSuit.Value == c.Suit)
                    return c;
            }
            //WILL NEVER HAPPEN
            return null;
        }

        public override Card ChooseTalon(Card trumps, Pile talon)
        {
            int[] strengths = new int[4];
            int[] marriage = new int[4];
            foreach (Card c in Hand.Enumerate())
            {
                strengths[c.Suit] += c.Value.Strength;
                if (c.Value.Marriage)
                    marriage[c.Suit]++;
            }
            strengths[trumps.Suit] = 999;
            IEnumerable<Card> sortedCards = Hand.Enumerate().Where(c => _ValidTalon(c, trumps))
                                                            .OrderBy(c => strengths[c.Suit])
                                                            .ThenBy(c => c.Value.Strength);
            bool IsNotMarried(Card c)
            {
                return !(c.Value.Marriage && marriage[c.Suit] == 2);
            }
            if (sortedCards.Any(IsNotMarried))
                return sortedCards.First(IsNotMarried);
            else
                return sortedCards.First();
        }
        public override Card ChoosePlay(List<Card> trick, Card trumps)
        {
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
            Card best = new Card(0);
            int bestRating = int.MinValue;
            foreach (Card c in Hand.Enumerate())
            {
                if (_ValidPlay(Hand, c, trumps.Suit, trick.ToArray(), trick.Count))
                {
                    int rating = rateOption(c, trick, trumps.Suit) - c.Value.Strength;
                    if (rating > bestRating)
                    {
                        best = c;
                        bestRating = rating;
                    }
                }
            }
            return best;
        }

        int RateFirstCardOffense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            //X - A risk -1M
            if (c.Value == Value.TEN)
            {
                if (others.ToPile().HasAny(new Card(c.Suit, Value.ACE)))
                {
                    rating -= 1_000_000;
                }
            }
            //A/X - trump risk -500K*expected
            float[] trumpedChances = new float[3];
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.Suit);
            if (c.Suit != trumps && others.ToPile().HasAny(trumps))
            {
                OtherPlayersSuitInfo trumpInfo = GetOtherPlayersSuitInfo(trumps);
                for (int j = 1; j <= 2; j++)
                {
                    trumpedChances[j] = suitInfo.noneChance[j] * (1 - trumpInfo.noneChance[j]);
                }
            }
            if (c.Value.GivesPoints)
            {
                rating -= (int)(500_000 * (trumpedChances[1] + trumpedChances[2]));
            }
            //A - X force take +20K*chance
            if (c.Value == Value.ACE)
            {
                if (others.ToPile().HasAny(new Card(c.Suit, Value.TEN)))
                {
                    float expected = 0;
                    for (int j = 1; j <= 2; j++)
                    {
                        if (!players[_PPlus(Index, j)].ToPile().HasAny(new Card(c.Suit, Value.TEN)))
                            continue;
                        float tenChance = 0;
                        if (!players[_PPlus(Index, 3 - j)].ToPile().HasAny(new Card(c.Suit, Value.TEN)))
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
            if (c.Suit == trumps)
                rating -= 5_000;
            //any - trump pull +1K*expected
            if (c.Suit == trumps)
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
            if (c.Value.GivesPoints)
            {
                if (prevPlayer.ToPile().HasAny(new Card(c.Suit, Value.ACE))
                    || (c.Suit != trumps && nextPlayer.ToPile().HasAny(trumps)
                    && nextPlayer.ToPile().Where(c.Suit).Count == others.ToPile().Where(c.Suit).Count))
                {
                    rating -= 10_000;
                }
            }
            //relative harm
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.Suit);
            rating += (int)(1000 * (suitInfo.noneChance[2] - suitInfo.noneChance[1]));
            //trump - -5K
            if (c.Suit == trumps)
                rating -= 5_000;
            return rating;
        }

        int RateFirstCardSecondDefense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(c.Suit);
            OtherPlayersSuitInfo trumpInfo = GetOtherPlayersSuitInfo(trumps);
            Pile guaranteed = ((Pile)others).Without(prevPlayer);
            Pile possible = ((Pile)nextPlayer).Without(guaranteed);
            //-10K if enemy could steal X or A
            if (c.Value.GivesPoints)
            {
                if (possible.HasAny(new Card(c.Suit, Value.ACE)) || (c.Suit != trumps && trumpInfo.max[1] > 0 && trumpInfo.min[1] <= 0))
                {
                    rating -= 10_000;
                }
            }
            //+10K * chance to steal X or A
            if (c.Suit != trumps)
            {
                if (!guaranteed.HasAny(c.SameSuitButLower.Without(Value.GivesPointsMask)))
                {
                    float stealChance = 0;
                    float noLowerChance = OneHalfPower(possible.Where(c.SameSuitButLower.Without(Value.GivesPointsMask)).Count);
                    if (guaranteed.HasAny(Value.GivesPointsMask.Where(c.Suit)))
                    {
                        float takeChance = 0;
                        if (possible.HasAny(new Card(c.Suit, Value.ACE)))
                        {
                            takeChance += 0.5f;
                        }
                        takeChance += suitInfo.noneChance[2] * (1 - trumpInfo.noneChance[2]);
                        stealChance += noLowerChance * takeChance;
                    }
                    else
                    {
                        if (possible.HasAny(new Card(c.Suit, Value.TEN)))
                        {
                            float takeChance = 0;
                            if (possible.HasAny(new Card(c.Suit, Value.ACE)))
                            {
                                takeChance += 0.5f;
                            }
                            takeChance += suitInfo.noneChance[2] * (1 - trumpInfo.noneChance[2]);
                            stealChance += noLowerChance * takeChance * 0.5f;
                        }
                        if (possible.HasAny(new Card(c.Suit, Value.ACE)))
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
            if (c.Suit == trumps)
                rating -= 5_000;
            return rating;
        }
        int RateSecondCard(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;

            Card first = trick[0];
            List<Card> updatedTrick = new List<Card> { first, c };
            //100 * excpected points * (win chance - loss chance)
            int pts = 10 * updatedTrick.Count(d => d.Value.GivesPoints);
            int bestSoFar = _TrickWinner(updatedTrick.ToArray(), trumps);
            Card best = updatedTrick[bestSoFar];

            float myWinChance;
            if (IsTrickIndexEnemyWhenImSecond(bestSoFar) == IsTrickIndexEnemyWhenImSecond(2))
            {
                myWinChance = IsTrickIndexEnemyWhenImSecond(bestSoFar) ? 0 : 1;
            }
            else
            {
                Pile guaranteed = ((Pile)others).Without(prevPlayer);
                Pile possible = ((Pile)nextPlayer).Without(guaranteed);
                float lastBeatsBestChance;
                OtherPlayersSuitInfo suitInfo = GetOtherPlayersSuitInfo(first.Suit);
                bool WillLastWinTrick(Card d)
                {
                    return _TrickWinner(new Card[] { first, c, d }, trumps) == 2;
                }
                int winningCards = possible.Enumerate().Count(d => first.SameSuit(d) && WillLastWinTrick(d));
                if (suitInfo.min[1] > 0)
                {
                    if (guaranteed.Enumerate().Any(d => first.SameSuit(d) && WillLastWinTrick(d)))
                    {
                        lastBeatsBestChance = 1;
                    }
                    else
                    {
                        lastBeatsBestChance = OneHalfPower(winningCards);
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
                    if (guaranteed.Enumerate().Any(d => d.Suit == trumps && WillLastWinTrick(d)))
                    {
                        lastBeatsBestChance = suitInfo.noneChance[2] + (1 - suitInfo.noneChance[2]) * beatsBestWithSuit;
                    }
                    else
                    {
                        int winningTrumpCards = possible.Enumerate().Count(d => d.Suit == trumps && WillLastWinTrick(d));
                        lastBeatsBestChance = suitInfo.noneChance[2] * (1 - OneHalfPower(winningTrumpCards)) + (1 - suitInfo.noneChance[2]) * beatsBestWithSuit;
                    }
                }
                if (!IsTrickIndexEnemyWhenImSecond(2))
                {
                    myWinChance = lastBeatsBestChance;
                }
                else if (!IsTrickIndexEnemyWhenImSecond(bestSoFar))
                {
                    myWinChance = 1 - lastBeatsBestChance;
                }
                else
                {
                    myWinChance = 0;
                }
            }
            rating += (int)(100 * (2 * myWinChance - 1) * (pts + 1));

            //penalty for playing A when X is not secured
            if (c.Value == Value.ACE && others.ToPile().HasAny(new Card(c.Suit, Value.TEN)))
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
            int winner = _TrickWinner(updatedTrick.ToArray(), trumps);
            if (winner == 2 || (!isOffense && (winner == (isFirstDefense ? 0 : 1))))
            {
                if (c.Value == Value.TEN && others.ToPile().HasAny(new Card(c.Suit, Value.ACE)))
                {
                    rating += 100_000;
                }
                else if (c.Value.GivesPoints && c.Suit != trumps)
                {
                    rating += 10_000 * (others.ToPile().Where(trumps).Count - us.ToPile().Where(trumps).Count);
                }
            }
            else if (c.Value.GivesPoints)
            {
                rating -= 1_000_000;
            }
            if (me.ToPile().HasAny(Value.GivesPointsMask.Where(c.Suit)))
            {
                rating += 100 * (me.ToPile().Where(c.Suit).Count) - 1_000;
            }
            else
            {
                rating += 1_000 - 100 * (me.ToPile().Where(c.Suit).Count);
            }
            return rating;
        }

        OtherPlayersSuitInfo GetOtherPlayersSuitInfo(Suit s)
        {
            int totalCards = others.ToPile().Where(s).Count;
            int[] maxCards = new int[3];
            for (int p = 1; p <= 2; p++)
            {
                maxCards[p] = players[_PPlus(Index, p)].ToPile().Where(s).Count;
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
