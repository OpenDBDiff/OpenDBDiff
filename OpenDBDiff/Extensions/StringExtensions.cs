using OpenDBDiff.Utils;
using ScintillaNET;
using System;
using System.Drawing;

namespace OpenDBDiff.Extensions
{
    public static class StringExtensions
    {
        public static char? CharAtOrDefault(this string value, int position, char? defaultValue = null)
        {
            if (value == null || position > value.Length - 1)
            {
                return defaultValue;
            }
            return value[position];
        }
    }
}
