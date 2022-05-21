namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftCurlyBracket : Delimiter
    {
        new public static string Literal => "{";

        public LeftCurlyBracket() : base(Literal) { }
    }
}
