namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class RightSquareBracket : Delimiter
    {
        new public static string Literal => "]";

        public RightSquareBracket(long startOffset) : base(startOffset, Literal) { }

        static RightSquareBracket() => ReservedTokens.RegisterToken(Literal, (pos) => new RightSquareBracket(pos));
    }
}
