using OCompiler.Utils;

namespace OCompiler.Analyze.Lexical.Tokens
{
    abstract class Token
    {
        public TokenPosition Position { get; protected set; } = new();
        public int Length => Literal.Length;

        public string Literal { get; }

        protected Token(string literal)
        {
            Literal = literal;
        }

        public static bool TryParse(TokenPosition position, string literal, out Token token)
        {
            if (ReservedTokens.IsReserved(literal))
            {
                var tokenConstructor = ReservedTokens.GetByLiteral(literal);
                token = tokenConstructor();
            }
            else if (literal.CanBeInteger())
            {
                token = new IntegerLiteral(literal);
            }
            else if (literal.CanBeDouble())
            {
                token = new RealLiteral(literal);
            }
            else if (literal.CanBeIdentifier())
            {
                token = new Identifier(literal);
            }
            else if (literal.IsWhitespace())
            {
                token = new Whitespace(literal);
            }
            else {
                token = new UnexistingToken();
            }

            token.Position = position;
            return token is not UnexistingToken;
        }
    }
}
