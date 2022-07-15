using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace ProjektLS22
{
    public static class Utils
    {

        public static Random _rand = new Random();
        public static void _Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                T c = list[k];
                list[k] = list[n];
                list[n] = c;
            }
        }

        public static void _Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive) ;
        }

        public static readonly string[] _playerNames = { "Jarda", "Franta", "Karel" };

        public static void _SortCards(ref List<Card> cards, Suit trumps, bool seven)
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
                return a.value.strength - b.value.strength;
            }));
        }

        static int CompareEquals(Object a, Object b, Object compareTo)
        {
            return CompareBool(a == compareTo, b == compareTo);
        }

        static int CompareBool(bool a, bool b)
        {
            if (a == b)
                return 0;
            return a ? -1 : 1;
        }

        public static bool _ValidTalon(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            Card c = hand[i];
            return c != trumps && !c.value.ten;
        }

        public static bool _ValidTrump(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            return i < 7;
        }

        public static bool _ValidPlay(List<Card> hand, int i, Card trumps, List<Card> trick)
        {
            if (trick.Count == 0)
                return true;
            Card selected = hand[i];
            Card first = trick[0];
            Card best = _BestFromFirstTwo(trumps, trick);
            if (hand.Exists(first.SameSuit))
            {
                return ValidPlayOn(hand, selected, first.suit, best);
            }
            else if (hand.Exists(trumps.SameSuit))
            {
                return ValidPlayOn(hand, selected, trumps.suit, best);
            }
            else
            {
                return true;
            }
        }

        public static Card _BestFromFirstTwo(Card trumps, List<Card> trick)
        {
            Card first = trick[0];
            if (trick.Count > 1)
            {
                Card second = trick[1];
                if (first.SameSuit(second))
                {
                    if (first.LowerThan(second))
                        return second;
                }
                else if (trumps.SameSuit(second))
                {
                    return second;
                }
            }
            return first;
        }

        static bool ValidPlayOn(List<Card> hand, Card selected, Suit suit, Card best)
        {
            if (suit.HasCard(best) && hand.Exists(best.SameSuitButLowerThan))
            {
                return best.SameSuitButLowerThan(selected);
            }
            else
            {
                return suit.HasCard(selected);
            }
        }

        public static int _TrickWinner(List<Card> trick, Suit trumps)
        {
            return trick.Select((c, i) => (((trumps.HasCard(c) ? 200 : (trick[0].SameSuit(c) ? 100 : 0)) + c.value.strength), i)).Max().i;
        }

        public static string _FormatCards(IEnumerable<Card> s, Suit trumps)
        {
            List<Card> l = new List<Card>(s);
            _SortCards(ref l, trumps, false);
            StringBuilder sb = new StringBuilder();
            Suit lastSuit = null;
            foreach (Card c in s)
            {
                if (c.suit != lastSuit)
                {
                    lastSuit = c.suit;
                    sb.Append(' ');
                    sb.Append(c.suit.prefix);
                }
                sb.Append(c.value.symbol);
            }
            return sb.ToString();
        }
        public static Func<int, int, int> _PPlus = (a, i) => (a + i) % 3;
    }
}