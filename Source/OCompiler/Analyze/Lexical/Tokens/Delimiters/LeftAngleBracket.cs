namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftAngleBracket : Delimiter
    {
        new public static string Literal => "<";

        public LeftAngleBracket() : base(Literal) { }
    }
}
