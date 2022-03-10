namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Var : Keyword
    {
        new public static string Literal => "var";

        public Var() : base(Literal) { }
    }
}
