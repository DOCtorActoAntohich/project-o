namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Base : Keyword
    {
        new public static string Literal => "base";

        public Base() : base(Literal) { }
    }
}
