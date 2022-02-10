namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Class : Keyword
    {
        new public static string Literal => "class";

        public Class(long startOffset) : base(startOffset, Literal) { }

        static Class() => ReservedTokens.RegisterToken(Literal, (pos) => new Class(pos));
    }
}
