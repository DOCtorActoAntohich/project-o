namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Class : Keyword
    {
        new public static string Literal => "class";

        public Class() : base(Literal) { }
    }
}
