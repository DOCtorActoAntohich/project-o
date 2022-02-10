namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class While : Keyword
    {
        new public static string Literal => "while";

        public While(long startOffset) : base(startOffset, Literal) { }

        static While() => ReservedTokens.RegisterToken(Literal, (pos) => new While(pos));
    }
}
