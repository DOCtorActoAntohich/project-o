namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class While : Keyword
    {
        new public static string Literal => "while";

        public While() : base(Literal) { }
    }
}
