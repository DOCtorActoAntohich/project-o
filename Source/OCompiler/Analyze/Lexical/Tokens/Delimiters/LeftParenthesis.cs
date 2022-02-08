namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftParenthesis : Delimiter
    {
        new public static string Literal => "(";

        public LeftParenthesis(long startOffset) : base(startOffset, Literal) { }

        static LeftParenthesis() => ReservedTokens.RegisterToken(Literal, (pos) => new LeftParenthesis(pos));
    }
}
