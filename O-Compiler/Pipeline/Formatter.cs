using System;
using System.Collections.Generic;

using OCompiler.Tokens;

namespace OCompiler.Pipeline
{
    internal static class Formatter
    {
        private static Dictionary<TokenType, ConsoleColor> Colors { get; } = new Dictionary<TokenType, ConsoleColor>() {
            { TokenType.Unknown,        ConsoleColor.Red      },
            { TokenType.Identifier,     ConsoleColor.Green    },
            { TokenType.RealLiteral,    ConsoleColor.White    },
            { TokenType.ReservedWord,   ConsoleColor.DarkCyan },
            { TokenType.IntegerLiteral, ConsoleColor.White    },
            { TokenType.BooleanLiteral, ConsoleColor.Cyan     },
        };

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
                Console.Write($"{token.StartOffset}-{token.EndOffset}:\t");
                Console.ForegroundColor = GetConsoleColor(token);
                if (token.Type != TokenType.Whitespace)
                {
                    Console.Write(token.Literal);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\t");
                Console.WriteLine(token.Type);
            }
        }

        private static ConsoleColor GetConsoleColor(Token token)
        {
            if (Colors.TryGetValue(token.Type, out ConsoleColor color))
            {
                return color;
            }
            return ConsoleColor.Gray;
        }
    }
}
