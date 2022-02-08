namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Is : Keyword
    {
        new public static string Literal => "is";

        public Is(long startOffset) : base(startOffset, Literal) { }

        static Is() => ReservedTokens.RegisterToken(Literal, (pos) => new Is(pos));
    }
}
