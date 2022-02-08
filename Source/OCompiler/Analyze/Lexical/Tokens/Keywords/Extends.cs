namespace OCompiler.Analyze.Lexical.Tokens.Keywords
{
    internal class Extends : Keyword
    {
        new public static string Literal => "extends";

        public Extends(long startOffset) : base(startOffset, Literal) { }

        static Extends() => ReservedTokens.RegisterToken(Literal, (pos) => new Extends(pos));
    }
}
