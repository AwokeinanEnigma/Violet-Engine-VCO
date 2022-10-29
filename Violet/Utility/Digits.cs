using System;

namespace Violet.Utility
{
    /// <summary>
    /// Class providing functions to help with number and digits
    /// </summary>
    public class Digits
    {
        public static int Get(int number, int place)
        {
            return Math.Abs(number) / (int)Math.Pow(10.0, place - 1) % 10;
        }

        /// <summary>
        /// Returns the number of digitals in an integer. 
        /// </summary>
        /// <param name="number">The number to get the digit count of</param>
        /// <returns>The number of digits in a number</returns>
        public static int CountDigits(int number)
        {
            int result = 1;
            if (number != 0)
            {
                result = (int)(Math.Log10(Math.Abs(number)) + 1.0);
            }
            return result;
        }
    }
}
