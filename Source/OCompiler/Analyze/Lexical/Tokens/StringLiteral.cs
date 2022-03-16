namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class StringLiteral : Token
    {
        public string EscapedLiteral => Literal.Replace(
            Delimiters.StringQuote.Literal,
            Delimiters.StringQuoteEscape.Literal
        );

        public StringLiteral(TokenPosition position, string literal) : base(literal) {
            Position = position;
        }
    }
}
