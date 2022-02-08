namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Assign : Delimiter
    {
        new public static string Literal => ":=";

        public Assign(long startOffset) : base(startOffset, Literal) { }

        static Assign() => ReservedTokens.RegisterToken(Literal, (pos) => new Assign(pos));
    }
}
