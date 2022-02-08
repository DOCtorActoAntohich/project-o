using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    abstract class Token
    {
        public long StartOffset { get; }
        public int Length => Literal.Length;

        public string Literal { get; }

        public Token(long startOffset, string literal)
        {
            StartOffset = startOffset;
            Literal = literal;
        }

        public static bool TryParse(long position, string literal, out Token token)
        {
            if (ReservedTokens.IsReserved(literal))
            {
                var tokenConstructor = ReservedTokens.GetByLiteral(literal);
                token = tokenConstructor(position);
                return true;
            }
            else if (literal.CanBeInteger())
            {
                token = new IntegerLiteral(position, literal);
                return true;
            }
            else if (literal.CanBeDouble())
            {
                token = new RealLiteral(position, literal);
                return true;
            }
            else if (literal.CanBeIdentifier())
            {
                token = new Identifier(position, literal);
                return true;
            }
            else if (literal.IsWhitespace())
            {
                token = new Whitespace(position, literal);
                return true;
            }
            token = new UnexistingToken(position);
            return false;
        }
    }
}
