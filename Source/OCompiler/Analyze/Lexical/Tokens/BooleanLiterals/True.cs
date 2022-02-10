namespace OCompiler.Analyze.Lexical.Tokens.BooleanLiterals
{
    internal class True : BooleanLiteral
    {
        new public static string Literal => "true";

        public True(long startOffset) : base(startOffset, Literal) { }

        static True() => ReservedTokens.RegisterToken(Literal, (pos) => new True(pos));
    }
}
