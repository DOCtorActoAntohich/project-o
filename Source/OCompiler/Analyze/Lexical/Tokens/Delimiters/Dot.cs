namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Dot : Delimiter
    {
        new public static string Literal => ".";

        public Dot(long startOffset) : base(startOffset, Literal) { }

        static Dot() => ReservedTokens.RegisterToken(Literal, (pos) => new Dot(pos));
    }
}
