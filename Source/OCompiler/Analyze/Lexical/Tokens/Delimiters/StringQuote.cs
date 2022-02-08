namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class StringQuote : Delimiter
    {
        new public static string Literal => "\"";

        public StringQuote(long startOffset) : base(startOffset, Literal) { }

        static StringQuote() => ReservedTokens.RegisterToken(Literal, (pos) => new StringQuote(pos));
    }
}
