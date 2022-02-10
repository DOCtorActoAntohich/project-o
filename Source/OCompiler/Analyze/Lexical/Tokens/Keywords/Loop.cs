namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Loop : Keyword
    {
        new public static string Literal => "loop";

        public Loop(long startOffset) : base(startOffset, Literal) { }

        static Loop() => ReservedTokens.RegisterToken(Literal, (pos) => new Loop(pos));
    }
}
