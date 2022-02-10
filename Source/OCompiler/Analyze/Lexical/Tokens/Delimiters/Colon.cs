namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Colon : Delimiter
    {
        new public static string Literal => ":";

        public Colon(long startOffset) : base(startOffset, Literal) { }

        static Colon() => ReservedTokens.RegisterToken(Literal, (pos) => new Colon(pos));
    }
}
