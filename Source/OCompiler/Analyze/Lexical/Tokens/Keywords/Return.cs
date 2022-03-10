namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Return : Keyword
    {
        new public static string Literal => "return";

        public Return() : base(Literal) { }
    }
}
