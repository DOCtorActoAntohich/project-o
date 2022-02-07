using System;
using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Pipeline
{
    internal static class Formatter
    {
        public static void ShowHighlightedCode(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Console.ForegroundColor = GetConsoleColor(token);
                var tokenOutput = token is StringLiteral stringToken ? $"\"{stringToken.EscapedLiteral}\"" : token.Literal;
                Console.Write(tokenOutput);
            }
            Console.WriteLine();
        }

        public static void ShowTokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Console.Write($"{token.StartOffset}-{token.StartOffset + token.Length}:".PadLeft(10));
                Console.ForegroundColor = GetConsoleColor(token);
                var tokenOutput = token is Whitespace or EndOfFile ? "" : $" '{token.Literal}'";
                Console.Write(tokenOutput.PadRight(15));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(token.GetType().Name.PadRight(15));
            }
        }

        private static ConsoleColor GetConsoleColor(Token token) => token switch
        {
            Keyword        => ConsoleColor.DarkCyan,
            Identifier     => ConsoleColor.Green,
            RealLiteral    => ConsoleColor.White,
            IntegerLiteral => ConsoleColor.White,
            BooleanLiteral => ConsoleColor.Cyan,
            StringLiteral  => ConsoleColor.Magenta,
            Delimiter      => ConsoleColor.Gray,
            Whitespace     => ConsoleColor.Gray,
            EndOfFile      => ConsoleColor.Gray,
            _ => ConsoleColor.Red,
        };
    }
}
