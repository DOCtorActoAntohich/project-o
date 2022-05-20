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
    internal class DictDefinition : Expression
    {
        public Dictionary<Expression, Expression> Items { get; set; } = new();
        public DictDefinition(Token firstToken, Expression? child = null, Expression? parent = null) : base(firstToken, child, parent)
        {
        }

        public static bool TryParse(TokenEnumerator tokens, out DictDefinition? dict)
        {
            var firstToken = tokens.Current();
            if (firstToken is not LeftCurlyBracket)
            {
                dict = null;
                return false;
            }

            tokens.Next();

            var items = new Dictionary<Expression, Expression>();
            while (tokens.Current() is not RightCurlyBracket)
            {
                if (!Expression.TryParse(tokens, out var keyExpression))
                {
                    throw new SyntaxError(tokens.Current().Position, "Expected an expression for a dictionary key");
                };
                if (tokens.Current() is not Colon)
                {
                    throw new SyntaxError(tokens.Current().Position, "Expected a colon after the dictionary key");
                };
                tokens.Next();

                if (!Expression.TryParse(tokens, out var valueExpression))
                {
                    throw new SyntaxError(tokens.Current().Position, "Expected an expression for a dictionary value");
                };

                switch (tokens.Current())
                {
                    case Comma:
                        tokens.Next();
                        break;
                    case not RightCurlyBracket:
                        throw new SyntaxError(tokens.Current().Position, "Expected a comma or the end of dictionary");
                }

                items.Add(keyExpression!, valueExpression!);
            }

            tokens.Next();
            dict = new(firstToken);
            dict.Items = items;

            return true;
        }

        protected override string SelfToString() => $"{{{string.Join(", ", Items)}}}";
    }
}
