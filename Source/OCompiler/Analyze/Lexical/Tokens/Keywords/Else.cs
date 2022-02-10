namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Else : Keyword
    {
        new public static string Literal => "else";

        public Else(long startOffset) : base(startOffset, Literal) { }

        static Else() => ReservedTokens.RegisterToken(Literal, (pos) => new Else(pos));
    }
}
