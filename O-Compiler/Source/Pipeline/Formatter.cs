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
                Console.Write(token.Literal);
            }
            Console.WriteLine();
        }

        public static void ShowTokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Console.Write($"{token.StartOffset}-{token.StartOffset + token.Length}:\t");
                Console.ForegroundColor = GetConsoleColor(token);
                if (token is not Whitespace)
                {
                    Console.Write(token.Literal);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\t");
                Console.WriteLine(token.GetType().Name);
            }
        }

        private static ConsoleColor GetConsoleColor(Token token) => token switch
        {
            Keyword        => ConsoleColor.DarkCyan,
            Identifier     => ConsoleColor.Green,
            RealLiteral    => ConsoleColor.White,
            IntegerLiteral => ConsoleColor.White,
            BooleanLiteral => ConsoleColor.Cyan,
            Delimiter      => ConsoleColor.Gray,
            Whitespace     => ConsoleColor.Gray,
            EndOfFile      => ConsoleColor.Gray,
            _ => ConsoleColor.Red,
        };
    }
}
