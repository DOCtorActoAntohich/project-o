namespace OCompiler.Analyze.Lexical.Tokens.Delimiters
{
    internal class Assign : Delimiter
    {
        new public static string Literal => ":=";

        public Assign() : base(Literal) { }
    }
}
