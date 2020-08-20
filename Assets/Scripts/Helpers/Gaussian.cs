using System;

namespace Iam.Scripts.Helpers
{
    public static class Gaussian
    {
        public static double NextGaussianDouble()
        {
            double u1 = 1.0 - UnityEngine.Random.value; //uniform(0,1] random doubles
            double u2 = 1.0 - UnityEngine.Random.value;
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        }
    }
}
