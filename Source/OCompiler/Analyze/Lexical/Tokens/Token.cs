using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    abstract class Token
    {
        public long StartOffset { get; }
        public int Length { get => Literal.Length; }

        public string Literal { get; }

        public Token(long startOffset, string literal)
        {
            StartOffset = startOffset;
            Literal = literal;
        }

        public static Token FromReserved(long position, Literals.ReservedLiteral literal) => literal switch
        {
            Literals.Boolean          => new BooleanLiteral(position, literal.Value),
            Literals.Delimiter        => new Delimiter(position, literal.Value),
            Literals.Keyword          => new Keyword(position, literal.Value),
            Literals.CommentDelimiter => new CommentDelimiter(position, literal.Value),
            _ => throw new System.ArgumentException("Argument passed is not a reserved literal")
        };

        public static bool TryParse(long position, string literal, out Token token)
        {
            if (literal.TryGetReservedLiteral(out var reservedLiteral))
            {
                token = FromReserved(position, reservedLiteral);
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
            token = null;
            return false;
        }
    }
}
