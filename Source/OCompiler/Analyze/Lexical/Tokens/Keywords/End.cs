namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class End : Keyword
    {
        new public static string Literal => "end";

        public End(long startOffset) : base(startOffset, Literal) { }

        static End() => ReservedTokens.RegisterToken(Literal, (pos) => new End(pos));
    }
}
