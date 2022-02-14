namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Loop : Keyword
    {
        new public static string Literal => "loop";

        public Loop() : base(Literal) { }
    }
}
