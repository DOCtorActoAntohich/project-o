namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class If : Keyword
    {
        new public static string Literal => "if";

        public If(long startOffset) : base(startOffset, Literal) { }

        static If() => ReservedTokens.RegisterToken(Literal, (pos) => new If(pos));
    }
}
