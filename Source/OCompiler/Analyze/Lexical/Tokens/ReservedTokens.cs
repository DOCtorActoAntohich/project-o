using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCompiler.Analyze.Lexical.Tokens
{
    static class ReservedTokens
    {
        private static Dictionary<string, Func<long, Token>> TokenConstructors { get; } = new();

        public static void RegisterToken(string literal, Func<long, Token> constructor)
        {
            TokenConstructors.TryAdd(literal, constructor);
        }

        static ReservedTokens()
        {
            // This will call all the static constructors of the reserved tokens,
            // which will add them to the TokenConstructors list.
            var literals = new string[] {
                BooleanLiterals.True.Literal,
                BooleanLiterals.False.Literal,
                CommentDelimiters.BlockEnd.Literal,
                CommentDelimiters.BlockStart.Literal,
                CommentDelimiters.LineStart.Literal,
                Delimiters.Dot.Literal,
                Delimiters.Colon.Literal,
                Delimiters.Comma.Literal,
                Delimiters.Assign.Literal,
                Delimiters.StringQuote.Literal,
                Delimiters.StringQuoteEscape.Literal,
                Delimiters.LeftParenthesis.Literal,
                Delimiters.LeftSquareBracket.Literal,
                Delimiters.RightParenthesis.Literal,
                Delimiters.RightSquareBracket.Literal,
                Keywords.Var.Literal,
                Keywords.If.Literal,
                Keywords.Then.Literal,
                Keywords.Else.Literal,
                Keywords.Class.Literal,
                Keywords.Extends.Literal,
                Keywords.This.Literal,
                Keywords.Method.Literal,
                Keywords.Is.Literal,
                Keywords.End.Literal,
                Keywords.Return.Literal,
                Keywords.While.Literal,
                Keywords.Loop.Literal,
            };
            if (literals.Length != TokenConstructors.Count)
            {
                throw new Exception("Some of reserved tokens have not been registered.");
            }
        }

        public static Func<long, Token> GetByLiteral(string literal)
        {
            return TokenConstructors.GetValueOrDefault(literal, pos => new UnexistingToken(pos));
        }

        public static bool IsReserved(string literal)
        {
            return TokenConstructors.ContainsKey(literal);
        }
    }
}
