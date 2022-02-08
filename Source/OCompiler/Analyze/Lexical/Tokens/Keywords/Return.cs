namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Return : Keyword
    {
        new public static string Literal => "return";

        public Return(long startOffset) : base(startOffset, Literal) { }

        static Return() => ReservedTokens.RegisterToken(Literal, (pos) => new Return(pos));
    }
}
