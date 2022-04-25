using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjektLS22
{
    public static class Utils
    {

        static Random rand = new Random();
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
    }
}