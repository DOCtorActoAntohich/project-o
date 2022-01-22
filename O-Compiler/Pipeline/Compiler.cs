using System;
using System.Collections.Generic;

using OCompiler.Tokens;

namespace OCompiler.Pipeline
{
    internal class Compiler
    {
        public string SourceFilePath { get; }
        public Compiler(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        public void Run()
        {
            // This is the only reference to ReservedWord for now
            // Required to prevent skipping the constructor of the class
            ReservedWord.Loop.ToString();

            var tokenizer = new Tokenizer(SourceFilePath);
            var tokens = tokenizer.GetTokens();
            ShowHighlightedCode(tokens);
        }

        private static void ShowHighlightedCode(List<Token> tokens)
        {

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Identifier:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case TokenType.ReservedWord:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case TokenType.Delimiter:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case TokenType.BooleanLiteral:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case TokenType.IntegerLiteral:
                    case TokenType.RealLiteral:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case TokenType.Unknown:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        break;
                }
                Console.Write(token.Literal);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
