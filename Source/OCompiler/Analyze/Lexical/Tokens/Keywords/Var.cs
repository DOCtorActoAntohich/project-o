namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Var : Keyword
    {
        new public static string Literal => "var";

        public Var(long startOffset) : base(startOffset, Literal) { }

        static Var() => ReservedTokens.RegisterToken(Literal, (pos) => new Var(pos));
    }
}
