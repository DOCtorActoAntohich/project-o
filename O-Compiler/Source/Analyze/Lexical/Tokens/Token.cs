using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    abstract class Token
    {
        public long StartOffset { get; }
        public int Length { get; }

        public string Literal { get; }

        public Token(long startOffset, string literal)
        {
            StartOffset = startOffset;
            Length = literal.Length;
            Literal = literal;
        }

        public static Token FromReserved(long position, Literals.ReservedLiteral literal) => literal switch
        {
            Literals.Boolean   => new BooleanLiteral(position, literal.Value),
            Literals.Delimiter => new Delimiter(position, literal.Value),
            Literals.Keyword   => new Keyword(position, literal.Value),
            _ => throw new System.ArgumentException("Argument passed is not a reserved literal")
        };

        public static bool TryParse(long position, string literal, out Token token)
        {
            var reservedLiteral = Literals.ReservedLiteral.GetByValue(literal);
            if (reservedLiteral == Literals.ReservedLiteral.Empty)
            {
                // Might be an identifier or Integer/Real literal
                if (literal[0].IsIdentifierOrNumber())
                {
                    if (char.IsDigit(literal[0]) && int.TryParse(literal, out int _))
                    {
                        token = new IntegerLiteral(position, literal);
                        return true;
                    }
                    else if (literal.Contains('.'))
                    {
                        if (literal.ToDouble(out double _))
                        {
                            token = new RealLiteral(position, literal);
                            return true;
                        }
                    }
                    else
                    {
                        token = new Identifier(position, literal);
                        return true;
                    }
                }
                else if (string.IsNullOrWhiteSpace(literal))
                {
                    token = new Whitespace(position, literal);
                    return true;
                }
            }
            else
            {
                token = FromReserved(position, reservedLiteral);
                return true;
            }
            token = null;
            return false;
        }
    }
}
