using System;
using System.Collections.Generic;

namespace LibPrint
{
    public static class Extensions
    {
        public static IEnumerable<string> Split(this string str, int n)
        {
            if (String.IsNullOrEmpty(str) || n < 1)
            {
                throw new ArgumentException();
            }

            int maxLong = (int)(str.Length / n) + 1;
            str = str.PadRight(maxLong*n, ' ');

            for (int i = 0; i < str.Length; i += n)
            {
                yield return str.Substring(i, n);
            }
        }
    }
}
