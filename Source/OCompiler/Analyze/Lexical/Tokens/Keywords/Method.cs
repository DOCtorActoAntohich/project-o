namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Method : Keyword
    {
        new public static string Literal => "method";

        public Method() : base(Literal) { }
    }
}
