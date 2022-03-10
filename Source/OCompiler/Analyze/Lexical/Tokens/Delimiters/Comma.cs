namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Comma : Delimiter
    {
        new public static string Literal => ",";

        public Comma() : base(Literal) { }
    }
}
