namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class This : Keyword
    {
        new public static string Literal => "this";

        public This() : base(Literal) { }
    }
}
