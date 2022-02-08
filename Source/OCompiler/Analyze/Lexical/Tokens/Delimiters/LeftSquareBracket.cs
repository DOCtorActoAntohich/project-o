namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class LeftSquareBracket : Delimiter
    {
        new public static string Literal => "[";

        public LeftSquareBracket(long startOffset) : base(startOffset, Literal) { }

        static LeftSquareBracket() => ReservedTokens.RegisterToken(Literal, (pos) => new LeftSquareBracket(pos));
    }
}
