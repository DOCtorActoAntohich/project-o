namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Comma : Delimiter
    {
        new public static string Literal => ",";

        public Comma(long startOffset) : base(startOffset, Literal) { }

        static Comma() => ReservedTokens.RegisterToken(Literal, (pos) => new Comma(pos));
    }
}
