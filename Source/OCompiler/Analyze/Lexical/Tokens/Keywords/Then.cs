namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Then : Keyword
    {
        new public static string Literal => "then";

        public Then(long startOffset) : base(startOffset, Literal) { }

        static Then() => ReservedTokens.RegisterToken(Literal, (pos) => new Then(pos));
    }
}
