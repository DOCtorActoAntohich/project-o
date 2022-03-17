using System;
using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Pipeline
{
    internal static class Formatter
    {
        public static void ShowHighlightedCode(IEnumerable<Token> tokens, bool withHint = true)
        {
            if (withHint)
            {
                Console.WriteLine("Highlighted code:");
            }
            foreach (var token in tokens)
            {
                Console.ForegroundColor = GetConsoleColor(token);
                var tokenOutput = token is StringLiteral stringToken ? $"\"{stringToken.EscapedLiteral}\"" : token.Literal;
                Console.Write(tokenOutput);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void ShowTokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Console.Write($"{token.Position}:".PadRight(8));
                Console.ForegroundColor = GetConsoleColor(token);
                var tokenOutput = token is Whitespace or EndOfFile ? "" : $" '{token.Literal}'";
                Console.Write(tokenOutput.PadRight(15));
                Console.ResetColor();
                Console.WriteLine(token.GetType().Name.PadRight(15));
            }
        }

        public static void ShowAST(Analyze.Syntax.Tree tree, bool withHint = true)
        {
            if (withHint)
            {
                Console.WriteLine("Syntax tree:");
            }
            Console.WriteLine(tree.ToString());
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
