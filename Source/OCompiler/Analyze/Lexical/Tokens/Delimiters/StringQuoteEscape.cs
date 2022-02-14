namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class StringQuoteEscape : Delimiter
    {
        new public static string Literal => "\\\"";

        public StringQuoteEscape() : base(Literal) { }
    }
}
