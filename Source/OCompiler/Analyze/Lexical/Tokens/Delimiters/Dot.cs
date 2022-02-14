namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Dot : Delimiter
    {
        new public static string Literal => ".";

        public Dot() : base(Literal) { }
    }
}
