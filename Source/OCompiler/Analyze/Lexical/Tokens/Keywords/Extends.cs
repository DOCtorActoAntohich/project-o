namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Extends : Keyword
    {
        new public static string Literal => "extends";

        public Extends() : base(Literal) { }
    }
}
