namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class This : Keyword
    {
        new public static string Literal => "this";

        public This(long startOffset) : base(startOffset, Literal) { }

        static This() => ReservedTokens.RegisterToken(Literal, (pos) => new This(pos));
    }
}
