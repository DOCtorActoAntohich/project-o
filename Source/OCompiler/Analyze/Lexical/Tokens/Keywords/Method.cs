namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Method : Keyword
    {
        new public static string Literal => "method";

        public Method(long startOffset) : base(startOffset, Literal) { }

        static Method() => ReservedTokens.RegisterToken(Literal, (pos) => new Method(pos));
    }
}
