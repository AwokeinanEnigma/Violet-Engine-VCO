﻿namespace Violet.Utility
{
    public static class Hash
    {
        public static int Get(string input)
        {
            int num = 23;
            for (int i = 0; i < input.Length; i++)
            {
                num = num * 31 + input[i];
            }
            return num;
        }
    }
}