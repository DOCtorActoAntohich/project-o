namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightSquareBracket : Delimiter
    {
        new public static string Literal => "]";

        public RightSquareBracket() : base(Literal) { }
    }
}
