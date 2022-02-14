namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class StringQuote : Delimiter
    {
        new public static string Literal => "\"";

        public StringQuote() : base(Literal) { }
    }
}
