namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightParenthesis : Delimiter
    {
        new public static string Literal => ")";

        public RightParenthesis(long startOffset) : base(startOffset, Literal) { }

        static RightParenthesis() => ReservedTokens.RegisterToken(Literal, (pos) => new RightParenthesis(pos));
    }
}
