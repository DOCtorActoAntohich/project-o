namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class End : Keyword
    {
        new public static string Literal => "end";

        public End() : base(Literal) { }
    }
}
