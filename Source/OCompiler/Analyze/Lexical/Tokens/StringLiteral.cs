namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class StringLiteral : Token
    {
        public string EscapedLiteral => Literal.Replace(
            Delimiters.StringQuote.Literal,
            Delimiters.StringQuoteEscape.Literal
        );

        public StringLiteral(long startOffset, string literal) : base(literal) {
            StartOffset = startOffset;
        }
    }
}
