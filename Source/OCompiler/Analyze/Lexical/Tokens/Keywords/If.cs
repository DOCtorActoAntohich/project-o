namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class If : Keyword
    {
        new public static string Literal => "if";

        public If() : base(Literal) { }
    }
}
