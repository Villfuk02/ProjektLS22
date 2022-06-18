using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Xml;

namespace ProjektLS22
{
    public class SmartAI : PlayerController
    {
        SpecificPlayerModel[] playerModels = new SpecificPlayerModel[3];
        SpecificPlayerModel me;
        SpecificPlayerModel ally;
        PlayerModel us;
        PlayerModel others;
        int[] secondCardEnemies;
        SpecificPlayerModel P(int n)
        {
            return playerModels[n % 3];
        }

        bool isOffense;
        bool isFirstDefense;

        public SmartAI()
        {
            for (int i = 0; i < 3; i++)
            {
                playerModels[i] = new SpecificPlayerModel();
            }
        }

        public override void NewRound(int dealer)
        {
            isOffense = player.index == (dealer + 1) % 3;
            isFirstDefense = player.index == (dealer + 2) % 3;
            for (int i = 0; i < 3; i++)
            {
                playerModels[i].Reset();
            }
            me = playerModels[player.index];
            others = new JointPlayerModel(P(player.index + 1), P(player.index + 2));
            if (isOffense)
            {
                ally = null;
                us = me;
                secondCardEnemies = new int[] { 0, 2 };
            }
            else if (isFirstDefense)
            {
                ally = P(dealer);
                us = new JointPlayerModel(me, ally);
                secondCardEnemies = new int[] { 0 };
            }
            else
            {
                ally = P(dealer + 2);
                us = new JointPlayerModel(me, ally);
                secondCardEnemies = new int[] { 2 };
            }
        }
        public override void FirstTrickStart(Card trumps, bool fromPeople, List<Card> talonIfKnown)
        {
            if (isOffense)
            {
                for (int i = 1; i <= 2; i++)
                {
                    P(player.index + i).RemoveRange(talonIfKnown);
                }
            }
            else
            {
                ally.Remove(trumps);
            }
            for (int i = 1; i <= 2; i++)
            {
                P(player.index + i).RemoveRange(player.hand);
            }
            me.RemoveMatching((Card c) => !player.hand.Contains(c));
        }
        public override void PlaysCard(int p, Card c, List<Card> trick, Card t, bool marrige)
        {
            Suit trumps = t.suit;
            for (int i = 0; i < 3; i++)
            {
                P(p + i).Remove(c);
            }
            if (p != player.index)
            {
                if (marrige)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        P(p + i).Remove(c.value == Value.KRÁL ? new Card(c.suit, Value.SVRŠEK) : new Card(c.suit, Value.KRÁL));
                    }
                }
                if (trick.Count == 2)
                {
                    if (c.suit == trick[0].suit)
                    {
                        if (c.value.gameStrength < trick[0].value.gameStrength)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trick[0].suit && d.value.gameStrength > trick[0].value.gameStrength);
                        }
                    }
                    else
                    {
                        P(p).RemoveMatching((Card d) => d.suit == trick[0].suit);
                        if (c.suit != trumps)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trumps);
                        }
                    }
                }
                else if (trick.Count == 3)
                {
                    Card best = trick[0];
                    if (trick.Count > 1)
                    {
                        if (trick[1].suit == trick[0].suit)
                        {
                            if (trick[1].value.gameStrength > trick[0].value.gameStrength)
                                best = trick[1];
                        }
                        else if (trick[1].suit == trumps)
                        {
                            best = trick[1];
                        }
                    }
                    if (c.suit == trick[0].suit)
                    {
                        if (best.suit != trick[0].suit)
                        {
                            if (c.value.gameStrength < trick[0].value.gameStrength)
                            {
                                P(p).RemoveMatching((Card d) => d.suit == c.suit && d.value.gameStrength > best.value.gameStrength);
                            }
                        }
                    }
                    else
                    {
                        P(p).RemoveMatching((Card d) => d.suit == trick[0].suit);
                        if (c.suit != trumps)
                        {
                            P(p).RemoveMatching((Card d) => d.suit == trumps);
                        }
                        else
                        {
                            if (best.suit == trumps && c.value.gameStrength < best.value.gameStrength)
                            {
                                P(p).RemoveMatching((Card d) => d.suit == trumps && d.value.gameStrength > best.value.gameStrength);
                            }
                        }
                    }
                }
            }
        }
        public override void TakesTrick(int p, List<Card> trick)
        {
        }
        public override int ChooseTrumps()
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>();
            for (int i = 0; i < 7; i++)
            {
                Card c = player.hand[i];
                if (strengths.ContainsKey(c.suit))
                    strengths[c.suit] += c.value.gameStrength;
                else
                    strengths[c.suit] = c.value.gameStrength;
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
                if (player.hand[i].suit == max.Key)
                    return i;
            }
            //SHOULD NEVER HAPPEN
            Renderer.PRINT.P($" {Utils.TranslatePlayer(player.index)} trump choice error ");
            Renderer.PrintHand(player.hand, true, false);
            return -2;
        }

        public override int ChooseTalon(Card trumps, List<Card> talon)
        {
            Dictionary<Suit, int> strengths = new Dictionary<Suit, int>();
            HashSet<Suit> married = new HashSet<Suit>();
            for (int i = 0; i < player.hand.Count; i++)
            {
                Card c = player.hand[i];
                if (i != 0 && c.value.marriage && player.hand[i - 1].value.marriage && c.suit == player.hand[i - 1].suit)
                {
                    married.Add(c.suit);
                }
                if (strengths.ContainsKey(c.suit))
                    strengths[c.suit] += c.value.gameStrength;
                else
                    strengths[c.suit] = c.value.gameStrength;
            }
            strengths[trumps.suit] = 999;
            var sorted = from entry in strengths orderby entry.Value ascending select entry.Key;
            foreach (Suit s in sorted)
            {
                for (int i = 0; i < player.hand.Count; i++)
                {
                    Card c = player.hand[i];
                    if (c.suit == s)
                    {
                        if (Utils.ValidTalon(player.hand, i, trumps, null))
                        {
                            if (c.value.marriage && married.Contains(s))
                            {
                                continue;
                            }
                            return i;
                        }
                    }
                }
            }
            return Utils.rand.Next(player.hand.Count);
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
                        {
                            rateOption = this.RateFirstCardOffense;
                        }
                        else if (isFirstDefense)
                        {
                            rateOption = this.RateFirstCardFirstDefense;
                        }
                        else
                        {
                            rateOption = this.RateFirstCardSecondDefense;
                        }
                        break;
                    }
                case 1:
                    {
                        rateOption = this.RateSecondCard;
                        break;
                    }
                case 2:
                    {
                        rateOption = this.RateLastCard;
                        break;
                    }
            }

            for (int i = 0; i < player.hand.Count; i++)
            {
                Card c = player.hand[i];
                //discourage playing high cards
                ratings[i] -= c.value.gameStrength;
                //block invalid cards
                if (!Utils.ValidPlay(player.hand, i, trumps, trick))
                {
                    ratings[i] = -1_000_000_000;
                }
                else
                {
                    ratings[i] += rateOption(c, trick, trumps.suit);
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
                if (others.MayHave().Contains(new Card(c.suit, Value.ESO)))
                {
                    rating -= 1_000_000;
                }
            }
            //A/X - trump risk -500K*expected
            float[] trumpedChances = new float[3];
            (int totalCards, int[] maxCards, int[] minCards, float[] noCardChances) = CountOtherPlayerCardsBySuit(c.suit);
            if (c.suit != trumps && others.MayHave().Any((Card d) => d.suit == trumps))
            {
                (int totalTrumpCards, int[] maxTrumpCards, int[] minTrumpCards, float[] noTrumpCardChances) = CountOtherPlayerCardsBySuit(trumps);
                for (int j = 1; j <= 2; j++)
                {
                    trumpedChances[j] = noCardChances[j] * (1 - noTrumpCardChances[j]);
                }
            }
            if (c.value.ten)
            {
                rating -= (int)(500_000 * (trumpedChances[1] + trumpedChances[2]));
            }
            //A - X force take +20K*chance
            if (c.value == Value.ESO)
            {
                if (others.MayHave().Contains(new Card(c.suit, Value.DESET)))
                {
                    float expected = 0;
                    for (int j = 1; j <= 2; j++)
                    {
                        if (!P(player.index + j).MayHave().Contains(new Card(c.suit, Value.DESET)))
                            continue;
                        float tenChance = 0;
                        if (!P(player.index + 3 - j).MayHave().Contains(new Card(c.suit, Value.DESET)))
                        {
                            if (maxCards[j] == 1)
                            {
                                tenChance = 1;
                            }
                            else if (minCards[j] >= 2)
                            {
                                tenChance = 0;
                            }
                            else
                            {
                                tenChance = 1f / (1 << (maxCards[j] - 1));
                            }
                        }
                        else
                        {
                            tenChance = 1f / (1 << maxCards[j]);
                        }
                        expected += tenChance * (1 - trumpedChances[3 - j]);
                    }
                    rating += (int)(20_000 * expected);
                }
            }
            //trump - -5K
            if (c.suit == trumps)
                rating -= 5_000;
            //any - trump pull +1K*expected
            if (c.suit == trumps)
            {
                rating += (int)(1_000 * (2 - noCardChances[1] - noCardChances[2]));
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
                if (P(player.index + 2).MayHave().Contains(new Card(c.suit, Value.ESO))
                    || (c.suit != trumps && P(player.index + 1).MayHave().Any((Card d) => d.suit == trumps)
                    && P(player.index + 1).MayHave().Count((Card d) => d.suit == c.suit) == others.MayHave().Count((Card d) => d.suit == c.suit)))
                {
                    rating -= 10_000;
                }
            }
            //relative harm
            (int totalCards, int[] maxCards, int[] minCards, float[] noCardChances) = CountOtherPlayerCardsBySuit(c.suit);
            rating += (int)(1000 * (noCardChances[2] - noCardChances[1]));
            //trump - -5K
            if (c.suit == trumps)
                rating -= 5_000;
            return rating;
        }

        int RateFirstCardSecondDefense(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            (int totalCards, int[] maxCards, int[] minCards, float[] noCardChances) = CountOtherPlayerCardsBySuit(c.suit);
            (int totalTrumpCards, int[] maxTrumpCards, int[] minTrumpCards, float[] noTrumpCardChances) = CountOtherPlayerCardsBySuit(trumps);
            HashSet<Card> guaranteed = new HashSet<Card>(others.MayHave());
            guaranteed.ExceptWith(P(player.index + 2).MayHave());
            HashSet<Card> possible = new HashSet<Card>(P(player.index + 1).MayHave());
            possible.ExceptWith(guaranteed);
            //-10K if enemy could steal X or A
            if (c.value.ten)
            {
                if (possible.Contains(new Card(c.suit, Value.ESO)) || (c.suit != trumps && maxTrumpCards[1] > 0 && minCards[1] <= 0))
                {
                    rating -= 10_000;
                }
            }
            //+10K * chance to steal X or A
            if (c.suit != trumps)
            {
                if (!guaranteed.Any((Card d) => d.suit == c.suit && d.value.gameStrength > c.value.gameStrength && !d.value.ten))
                {
                    float stealChance = 0;
                    float noLowerChance = 1f / (1 << possible.Count((Card d) => d.suit == c.suit && d.value.gameStrength > c.value.gameStrength && !d.value.ten));
                    if (guaranteed.Contains(new Card(c.suit, Value.DESET)) || guaranteed.Contains(new Card(c.suit, Value.ESO)))
                    {
                        float takeChance = 0;
                        if (possible.Contains(new Card(c.suit, Value.ESO)))
                        {
                            takeChance += 0.5f;
                        }
                        takeChance += noCardChances[2] * (1 - noTrumpCardChances[2]);
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
                            takeChance += noCardChances[2] * (1 - noTrumpCardChances[2]);
                            stealChance += noLowerChance * takeChance * 0.5f;
                        }
                        if (possible.Contains(new Card(c.suit, Value.ESO)))
                        {
                            float takeChance = 0;
                            takeChance += noCardChances[2] * (1 - noTrumpCardChances[2]);
                            stealChance += noLowerChance * takeChance * 0.5f;
                        }
                    }
                    rating += (int)(10_000 * stealChance);
                }
            }

            //1K * relative harm
            rating += (int)(1000 * (noCardChances[1] - noCardChances[2]));

            //trump - -5K
            if (c.suit == trumps)
                rating -= 5_000;
            return rating;
        }
        int RateSecondCard(Card c, List<Card> trick, Suit trumps)
        {
            int rating = 0;
            //100 * excpected points * (win chance - loss chance)
            List<Card> updatedTrick = new List<Card> { trick[0], c };
            int pts = 10 * updatedTrick.Count((Card d) => d.value.ten);
            int bestSoFar = Utils.TrickWinner(updatedTrick, trumps);
            Card best = updatedTrick[bestSoFar];

            float winChance;
            if (secondCardEnemies.Contains(bestSoFar) == secondCardEnemies.Contains(2))
            {
                winChance = secondCardEnemies.Contains(bestSoFar) ? 0 : 1;
            }
            else
            {
                HashSet<Card> guaranteed = new HashSet<Card>(others.MayHave());
                guaranteed.ExceptWith(P(player.index + 2).MayHave());
                HashSet<Card> possible = new HashSet<Card>(P(player.index + 1).MayHave());
                possible.ExceptWith(guaranteed);
                float beatsBestChance;
                (int totalCards, int[] maxCards, int[] minCards, float[] noCardChances) = CountOtherPlayerCardsBySuit(trick[0].suit);
                int winningCards = possible.Count((Card d) => d.suit == trick[0].suit && Utils.TrickWinner(new List<Card> { trick[0], c, d }, trumps) == 2);
                if (minCards[1] > 0)
                {
                    if (guaranteed.Any((Card d) => d.suit == trick[0].suit && Utils.TrickWinner(new List<Card> { trick[0], c, d }, trumps) == 2))
                    {
                        beatsBestChance = 1;
                    }
                    else
                    {

                        beatsBestChance = 1f / (1 << winningCards);
                    }
                }
                else
                {
                    float beatsBestWithSuit;
                    if (maxCards[2] > 0)
                    {
                        beatsBestWithSuit = 1 - ((1 << (maxCards[2] - winningCards)) - 1) / (float)((1 << maxCards[2]) - 1);
                    }
                    else
                    {
                        beatsBestWithSuit = 0;
                    }
                    if (guaranteed.Any((Card d) => d.suit == trumps && Utils.TrickWinner(new List<Card> { trick[0], c, d }, trumps) == 2))
                    {
                        beatsBestChance = noCardChances[2] + (1 - noCardChances[2]) * beatsBestWithSuit;
                    }
                    else
                    {
                        int winningTrumpCards = possible.Count((Card d) => d.suit == trumps && Utils.TrickWinner(new List<Card> { trick[0], c, d }, trumps) != 1);
                        beatsBestChance = noCardChances[2] * (1 - (1f / (1 << winningTrumpCards))) + (1 - noCardChances[2]) * beatsBestWithSuit;
                    }
                }
                winChance = 0;
                if (!secondCardEnemies.Contains(2))
                {
                    winChance += beatsBestChance;
                }
                if (!secondCardEnemies.Contains(bestSoFar))
                {
                    winChance += 1 - beatsBestChance;
                }
            }
            rating += (int)(100 * (2 * winChance - 1) * (pts + 1));

            //penalty for playing A when X is not secured
            if (c.value == Value.ESO && others.MayHave().Contains(new Card(c.suit, Value.DESET)))
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
            int winner = Utils.TrickWinner(updatedTrick, trumps);
            if (winner == 2 || (!isOffense && (winner == (isFirstDefense ? 0 : 1))))
            {
                if (c.value == Value.DESET && others.MayHave().Contains(new Card(c.suit, Value.ESO)))
                {
                    rating += 100_000;
                }
                else if (c.value.ten && c.suit != trumps)
                {
                    rating += 10_000 * (others.MayHave().Count((Card d) => d.suit == trumps) - us.MayHave().Count((Card d) => d.suit == trumps));
                }
            }
            else if (c.value.ten)
            {
                rating -= 1_000_000;
            }
            if (me.MayHave().Any((Card d) => d.suit == c.suit && d.value.ten))
            {
                rating += 100 * (me.MayHave().Count((Card d) => d.suit == c.suit)) - 1_000;
            }
            else
            {
                rating += 1_000 - 100 * (me.MayHave().Count((Card d) => d.suit == c.suit));
            }
            return rating;
        }

        (int, int[], int[], float[]) CountOtherPlayerCardsBySuit(Suit s)
        {
            int totalCards = others.MayHave().Count((Card d) => d.suit == s);
            int[] maxCards = new int[3];
            int[] minCards = new int[3];
            float[] noCardChances = new float[3];
            for (int p = 1; p <= 2; p++)
            {
                maxCards[p] = P(player.index + p).MayHave().Count((Card d) => d.suit == s);
            }
            for (int p = 1; p <= 2; p++)
            {
                minCards[p] = totalCards - maxCards[3 - p];
                if (minCards[p] <= 0)
                {
                    noCardChances[p] = 1f / (1 << maxCards[p]);
                }
            }
            return (totalCards, maxCards, minCards, noCardChances);
        }
    }
}
