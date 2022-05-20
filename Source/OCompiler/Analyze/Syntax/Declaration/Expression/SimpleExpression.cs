using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCompiler.Analyze.Syntax.Declaration.Expression
{
    internal class SimpleExpression : Expression
    {
        public SimpleExpression(Token name, Expression? child = null, Expression? parent = null) : base(name, child, parent)
        {
        }

        public static bool TryParse(TokenEnumerator tokens, out SimpleExpression? expression)
        {
            if (tokens.Current() is not (
                Identifier or
                StringLiteral or
                RealLiteral or
                IntegerLiteral or
                BooleanLiteral or
                Lexical.Tokens.Keywords.This or
                Lexical.Tokens.Keywords.Base
            ))
            {
                expression = null;
                return false;
            }

            // Parse token
            Token token = tokens.Current();
            // Get next token.
            tokens.Next();

            // Try parse arguments.
            if (Arguments.TryParse(tokens, out List<Expression>? arguments))
            {
                expression = new Call(token, arguments!);
            }
            else
            {
                expression = new SimpleExpression(token);
            }

            return true;
        }

        protected override string SelfToString()
        {
            if (Token is StringLiteral)
            {
                return $"\"{Token.Literal}\"";
            }

            return Token.Literal;
        }
    }
}
