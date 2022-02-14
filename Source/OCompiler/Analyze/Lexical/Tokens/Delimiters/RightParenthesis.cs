namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightParenthesis : Delimiter
    {
        new public static string Literal => ")";

        public RightParenthesis() : base(Literal) { }
    }
}
