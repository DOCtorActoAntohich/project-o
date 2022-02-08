namespace OCompiler.Analyze.Lexical.Tokens.BooleanLiterals
{
    internal class False : BooleanLiteral
    {
        new public static string Literal => "false";

        public False(long startOffset) : base(startOffset, Literal) { }

        static False() => ReservedTokens.RegisterToken(Literal, (pos) => new False(pos));
    }
}
