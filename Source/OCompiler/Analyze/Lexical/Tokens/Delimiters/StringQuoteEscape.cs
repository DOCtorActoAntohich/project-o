namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class StringQuoteEscape : Delimiter
    {
        new public static string Literal => "\\\"";

        public StringQuoteEscape(long startOffset) : base(startOffset, Literal) { }

        static StringQuoteEscape() => ReservedTokens.RegisterToken(Literal, (pos) => new StringQuoteEscape(pos));
    }
}
