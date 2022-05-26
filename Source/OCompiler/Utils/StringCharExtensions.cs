using System.Globalization;

namespace OCompiler.Utils
{
    internal static class StringCharExtensions
    {
        public static bool IsIdentifierOrNumber(this char c)
        {
            // '.' is included for Real numbers, '-' for negatives
            return char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-';
        }
        public static bool IsIdentifierOrNumber(this string s)
        {
            foreach (char c in s)
            {
                if (!c.IsIdentifierOrNumber())
                    return false;
            }
            return true;
        }
        public static bool CanBeIdentifier(this string s)
        {
            if (s.Length == 0 || !s[0].CanStartIdentifier())
            {
                return false;
            }
            foreach (char c in s)
            {
                if (!c.CanBePartOfIdentifier())
                    return false;
            }
            return true;
        }

        public static bool CanStartIdentifier(this char c)
        {
            return char.IsLetter(c) || c == '_';
        }
        public static bool CanBePartOfIdentifier(this char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        public static bool TryCastToDouble(this string literal, out double result)
        {
            return double.TryParse(
                literal, result: out result,
                style: NumberStyles.Any,
                provider: CultureInfo.InvariantCulture
            );
        }
        public static bool CanBeDouble(this string literal)
        {
            foreach (char c in literal)
            {
                if (!(char.IsDigit(c) || c == '.' || c == '-'))
                    return false;
            }
            return literal.TryCastToDouble(out double _);
        }

        public static bool TryCastToInteger(this string literal, out int conversion_result)
        {
            return int.TryParse(literal, out conversion_result);
        }
        public static bool CanBeInteger(this string literal)
        {
            foreach (char c in literal)
            {
                if (!(char.IsDigit(c) || c == '-'))
                    return false;
            }
            return literal.TryCastToInteger(out int _);
        }

        public static bool IsWhitespace(this string literal)
        {
            return literal.Length > 0 && string.IsNullOrWhiteSpace(literal);
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

        public static string RemoveSuffix(this string s, string suffix)
        {
            return s.EndsWith(suffix) ? s[..^suffix.Length] : s;
        }
    }
}
