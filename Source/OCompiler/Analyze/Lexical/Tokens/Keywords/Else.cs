namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Else : Keyword
    {
        new public static string Literal => "else";

        public Else() : base(Literal) { }
    }
}
