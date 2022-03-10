namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Is : Keyword
    {
        new public static string Literal => "is";

        public Is() : base(Literal) { }
    }
}
