namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightAngleBracket : Delimiter
    {
        new public static string Literal => ">";

        public RightAngleBracket() : base(Literal) { }
    }
}
