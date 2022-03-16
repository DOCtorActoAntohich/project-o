using System;
using System.Collections.Generic;

namespace OCompiler.Analyze.Lexical.Tokens
{
    static class ReservedTokens
    {
        private static Dictionary<string, Func<Token>> TokenConstructors { get; } = new();

        public static void RegisterToken<T>() where T : Token, new()
        {
            TokenConstructors.Add(new T().Literal, () => new T());
        }

        static ReservedTokens()
        {
            RegisterToken<BooleanLiterals.True>();
            RegisterToken<BooleanLiterals.False>();
            RegisterToken<CommentDelimiters.BlockEnd>();
            RegisterToken<CommentDelimiters.BlockStart>();
            RegisterToken<CommentDelimiters.LineStart>();
            RegisterToken<Delimiters.Dot>();
            RegisterToken<Delimiters.Colon>();
            RegisterToken<Delimiters.Comma>();
            RegisterToken<Delimiters.Assign>();
            RegisterToken<Delimiters.StringQuote>();
            RegisterToken<Delimiters.StringQuoteEscape>();
            RegisterToken<Delimiters.LeftParenthesis>();
            RegisterToken<Delimiters.LeftSquareBracket>();
            RegisterToken<Delimiters.RightParenthesis>();
            RegisterToken<Delimiters.RightSquareBracket>();
            RegisterToken<Keywords.Var>();
            RegisterToken<Keywords.If>();
            RegisterToken<Keywords.Then>();
            RegisterToken<Keywords.Else>();
            RegisterToken<Keywords.Class>();
            RegisterToken<Keywords.Extends>();
            RegisterToken<Keywords.Base>();
            RegisterToken<Keywords.This>();
            RegisterToken<Keywords.Method>();
            RegisterToken<Keywords.Is>();
            RegisterToken<Keywords.End>();
            RegisterToken<Keywords.Return>();
            RegisterToken<Keywords.While>();
            RegisterToken<Keywords.Loop>();
        }

        public static Func<Token> GetByLiteral(string literal)
        {
            return TokenConstructors.GetValueOrDefault(literal, () => new UnexistingToken());
        }

        public static bool IsReserved(string literal)
        {
            return TokenConstructors.ContainsKey(literal);
        }
    }
}
