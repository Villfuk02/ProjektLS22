using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjektLS22
{
    public static class Utils
    {

        public static Random rand = new Random();
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T c = list[k];
                list[k] = list[n];
                list[n] = c;
            }
        }

        public static void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive) ;
        }

        static string[] names = { "Jarda", "Franta", "Karel" };
        public static string TranslatePlayer(int num)
        {
            num = (num + 3) % 3;
            return names[num];
        }

        public static void SortCards(ref List<Card> cards, Suit trumps, bool seven)
        {
            cards.Sort(0, (seven ? 7 : cards.Count), Comparer<Card>.Create((Card a, Card b) =>
            {
                int res = 0;
                res = CompareEquals(a.suit, b.suit, trumps);
                if (res != 0) return res;
                foreach (Suit s in Suit.ALL)
                {
                    res = CompareEquals(a.suit, b.suit, s);
                    if (res != 0) return res;
                }
                return a.value.gameStrength - b.value.gameStrength;
            }));
        }

        public static int CompareEquals(Object a, Object b, Object compareTo)
        {
            return CompareBool(a == compareTo, b == compareTo);
        }

        public static int CompareBool(bool a, bool b)
        {
            if (a == b)
                return 0;
            return a ? -1 : 1;
        }

        public static bool ValidTalon(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            Card c = hand[i];
            return c != trumps && !c.value.ten;
        }

        public static bool ValidTrump(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            return i < 7;
        }

        public static bool ValidPlay(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            if (trick.Count == 0)
                return true;
            Card selected = hand[i];
            Card first = trick[0];
            Card best = first;
            if (trick.Count > 1)
            {
                if (trick[1].suit == first.suit)
                {
                    if (trick[1].value.gameStrength > first.value.gameStrength)
                        best = trick[1];
                }
                else if (trick[1].suit == trumps.suit)
                {
                    best = trick[1];
                }
            }
            if (hand.Exists((Card c) => c.suit == first.suit))
            {
                return ValidPlayOn(hand, selected, first.suit, best);
            }
            else if (hand.Exists((Card c) => c.suit == trumps.suit))
            {
                return ValidPlayOn(hand, selected, trumps.suit, best);
            }
            else
            {
                return true;
            }
        }

        static bool ValidPlayOn(List<Card> hand, Card selected, Suit suit, Card best)
        {
            if (suit == best.suit && hand.Exists((Card c) => c.suit == best.suit && c.value.gameStrength > best.value.gameStrength))
            {
                return selected.suit == best.suit && selected.value.gameStrength > best.value.gameStrength;
            }
            else
            {
                return selected.suit == suit;
            }
        }

        public static void PrintValidChoices(List<Card> cards, Card trumps, List<Card> trick, Func<List<Card>, int, Card, List<Card>, bool> validator)
        {
            Renderer.PRINT.H();
            for (int i = 0; i < cards.Count; i++)
            {
                if (validator(cards, i, trumps, trick))
                {
                    Renderer.PRINT.P(HumanPlayerController.cardChoiceLetters[i]);
                }
            }
            Renderer.PRINT.H();
        }
    }
}