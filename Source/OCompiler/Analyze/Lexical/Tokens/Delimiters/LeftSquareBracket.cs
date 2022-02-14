namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftSquareBracket : Delimiter
    {
        new public static string Literal => "[";

        public LeftSquareBracket() : base(Literal) { }
    }
}
