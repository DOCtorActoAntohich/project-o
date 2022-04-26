namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Equals : Delimiter
    {
        new public static string Literal => "=";

        public Equals() : base(Literal) { }
    }
}
