namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftParenthesis : Delimiter
    {
        new public static string Literal => "(";

        public LeftParenthesis() : base(Literal) { }
    }
}
