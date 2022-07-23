using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace ProjektLS22
{
    /// <summary>
    /// Contains utility functions used throughout the project.
    /// </summary>
    public static class Utils
    {

        public static Random _rand = new Random();
        /// <summary>
        /// Randomly shuffles given list.
        /// </summary>
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
        /// <summary>
        /// Stops the execution of this thread for a given time.
        /// </summary>
        /// <param name="time">time to wait in milliseconds</param>
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
        /// <summary>
        /// Sorts cards by suit and value, puts trumps in front.
        /// </summary>
        public static void _SortCards(ref List<Card> cards, Suit? trumps)
        {
            cards.Sort(Comparer<Card>.Create((Card a, Card b) =>
            {
                if (a.SameSuit(b))
                    return a.Value - b.Value;
                if (trumps.HasValue)
                {
                    if (a.Suit == trumps)
                        return -1;
                    if (b.Suit == trumps)
                        return 1;
                }
                return a.Suit - b.Suit;
            }));
        }
        /// <summary>
        /// Returns if a card can be selected as trumps. Input given in a format common to all "Validators".
        /// </summary>
        public static Func<Pile, Card, Card?, List<Card>, bool> _TrumpValidator = (_, _, _, _) => true;
        /// <summary>
        /// Returns if a card can be put in talon. Input given in a format common to all "Validators".
        /// </summary>
        public static Func<Pile, Card, Card?, List<Card>, bool> _TalonValidator = (_, c, t, _) => _ValidTalon(c, t.Value);
        /// <summary>
        /// Returns if a card can be played. Input given in a format common to all "Validators".
        /// </summary>
        public static Func<Pile, Card, Card?, List<Card>, bool> _PlayValidator = (h, c, trumps, trick) => _ValidPlay(h, c, trumps.Value.Suit, trick.ToArray(), trick.Count);
        /// <summary>
        /// Returns if a card can be put in talon.
        /// </summary>
        public static bool _ValidTalon(Card selection, Card trumps)
        {
            return selection != trumps && !selection.Value.GivesPoints;
        }
        /// <summary>
        /// Returns if a card can be played.
        /// </summary>
        public static bool _ValidPlay(Pile hand, Card selection, Suit trumps, Card[] trick, int trickCount)
        {
            if (trickCount == 0)
                return true;
            Card first = trick[0];
            Card best = _BestFromFirstTwo(trumps, trick, trickCount);
            if (hand.HasAny(first.Suit))
            {
                if (best.SameSuit(first) && hand.HasAny(best.SameSuitButHigher))
                {
                    return best.SameSuitButHigher.HasAny(selection);
                }
                else
                {
                    return first.SameSuit(selection);
                }
            }
            else if (hand.HasAny(trumps))
            {
                if (best.Suit == trumps && hand.HasAny(best.SameSuitButHigher))
                {
                    return best.SameSuitButHigher.HasAny(selection);
                }
                else
                {
                    return selection.Suit == trumps;
                }
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Returns the strongest card from up to the first two in a trick.
        /// </summary>
        public static Card _BestFromFirstTwo(Suit trumps, Card[] trick, int trickCount)
        {
            Card first = trick[0];
            if (trickCount > 1)
            {
                Card second = trick[1];
                if (first.SameSuit(second))
                {
                    if (first.Value < second.Value)
                        return second;
                }
                else if (second.Suit == trumps)
                {
                    return second;
                }
            }
            return first;
        }
        /// <summary>
        /// Decides which card is the strongest in a trick and returns its index.
        /// </summary>
        public static int _TrickWinner(Card[] trick, Suit trumps)
        {
            return trick.Select((c, i) => (((c.Suit == trumps ? 20 : (trick[0].SameSuit(c) ? 10 : 0)) + c.Value), i)).Max().i;
        }
        /// <summary>
        /// Formats a collection of cards into a nice string for simpler debugging.
        /// </summary>
        public static string _FormatCards(IEnumerable<Card> s, Suit? trumps)
        {
            List<Card> l = new List<Card>(s);
            _SortCards(ref l, trumps);
            StringBuilder sb = new StringBuilder();
            Suit? lastSuit = null;
            foreach (Card c in s)
            {
                if (!lastSuit.HasValue || c.Suit != lastSuit.Value)
                {
                    if (lastSuit.HasValue)
                        sb.Append(' ');
                    sb.Append(c.Suit.Prefix);
                    lastSuit = c.Suit;
                }
                sb.Append(c.Value.Symbol);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Adds a number to a player index mod 3. For example player 0 is right after player 2.
        /// </summary>
        public static Func<int, int, int> _PPlus = (a, i) => (a + i) % 3;
    }
}