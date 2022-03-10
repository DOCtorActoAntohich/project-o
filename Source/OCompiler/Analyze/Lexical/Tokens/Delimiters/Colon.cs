namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Colon : Delimiter
    {
        new public static string Literal => ":";

        public Colon() : base(Literal) { }
    }
}
