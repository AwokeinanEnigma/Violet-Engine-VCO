using System;

namespace Violet.Utility
{
    /// <summary>
    /// Class providing a generic gaussian random.
    /// </summary>
    public class GaussianRandom
    {
        /// <summary>
        /// Returns a random number
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stdDev"></param>
        /// <returns></returns>
        public static double Next(double mean, double stdDev)
        {
            double ndouble = Engine.Random.NextDouble();
            double num = Engine.Random.NextDouble();
            double num2 = Math.Sqrt(-2.0 * Math.Log(ndouble)) * Math.Sin(6.283185307179586 * num);
            return mean + stdDev * num2;
        }
    }
}
