using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjektLS22
{
    public static class Utils
    {

        public static Random rand = new Random();
        public static void Shuffle(this List<Card> cards)
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Card c = cards[k];
                cards[k] = cards[n];
                cards[n] = c;
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
            num = (num + GameSetup.PLAYER_AMT) % GameSetup.PLAYER_AMT;
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
    }
}