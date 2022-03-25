using System;

namespace ProjektLS22
{
    public static class Utils
    {

        static Random rand = new Random();
        public static (double x, double y) GetRandomPointInsideCircle(double radius)
        {
            radius *= Math.Max(rand.NextDouble(), rand.NextDouble());
            double angle = Math.PI * 2 * rand.NextDouble();
            return OnCircle(radius, angle);
        }

        public static (double x, double y) OnCircle(double radius, double angle)
        {
            return (radius * Math.Cos(angle), radius * Math.Sin(angle));
        }
    }
}