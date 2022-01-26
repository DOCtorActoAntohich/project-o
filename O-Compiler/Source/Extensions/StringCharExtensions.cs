using System.Globalization;

namespace OCompiler.Extensions
{
    internal static class StringCharExtensions
    {
        public static bool IsIdentifierOrNumber(this char c)
        {
            // '.' is included for Real numbers
            return char.IsLetterOrDigit(c) || c == '.' || c == '_';
        }

        public static bool ToDouble(this string literal, out double result)
        {
            return double.TryParse(
                literal, result: out result,
                style: NumberStyles.Any,
                provider: CultureInfo.InvariantCulture
            );
        }

        public static bool ToInteger(this string literal, out int result)
        {
            return int.TryParse(literal, out result);
        }

        public static int Count(this string haystack, char needle)
        {
            int occurences = 0;
            foreach (char c in haystack)
            {
                if (c == needle)
                {
                    occurences++;
                }
            }
            return occurences;
        }
    }
}
