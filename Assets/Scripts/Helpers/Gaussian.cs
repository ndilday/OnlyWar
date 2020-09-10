using System;

namespace Iam.Scripts.Helpers
{
    public static class Random
    {
        private static System.Random _random = new System.Random(1);
        public static double NextGaussianDouble()
        {
            double u1 = 1.0 - _random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - _random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        }

        public static int GetIntBelowMax(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
