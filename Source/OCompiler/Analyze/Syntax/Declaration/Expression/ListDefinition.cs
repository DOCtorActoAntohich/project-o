using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.Delimiters;
using OCompiler.Exceptions;
using OCompiler.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCompiler.Analyze.Syntax.Declaration.Expression
{
    internal class ListDefinition : Expression
    {
        public List<Expression> Items { get; set; } = new();
        public ListDefinition(Token firstToken, Expression? child = null, Expression? parent = null) : base(firstToken, child, parent)
        {
        }

        public static bool TryParse(TokenEnumerator tokens, out ListDefinition? list)
        {
            var firstToken = tokens.Current();
            if (firstToken is not LeftSquareBracket)
            {
                list = null;
                return false;
            }

            tokens.Next();

            var items = new List<Expression>();
            while (tokens.Current() is not RightSquareBracket)
            {
                if (!Expression.TryParse(tokens, out var expression))
                {
                    throw new SyntaxError(tokens.Current().Position, "Expected an expression");
                };
                
                switch (tokens.Current())
                {
                    case Comma:
                        tokens.Next();
                        break;
                    case not RightSquareBracket:
                        throw new SyntaxError(tokens.Current().Position, "Expected a comma or the end of list");
                }

                items.Add(expression!);
            }

            tokens.Next();
            list = new(firstToken);
            list.Items = items;

            return true;
        }

        protected override string SelfToString() => $"[{string.Join(", ", Items)}]";
    }
}
