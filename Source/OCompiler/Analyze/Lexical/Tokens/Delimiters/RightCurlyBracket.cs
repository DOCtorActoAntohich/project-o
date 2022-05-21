namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightCurlyBracket : Delimiter
    {
        new public static string Literal => "}";

        public RightCurlyBracket() : base(Literal) { }
    }
}
