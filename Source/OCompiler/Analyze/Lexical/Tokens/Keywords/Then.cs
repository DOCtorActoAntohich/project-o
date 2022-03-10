namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Then : Keyword
    {
        new public static string Literal => "then";

        public Then() : base(Literal) { }
    }
}
