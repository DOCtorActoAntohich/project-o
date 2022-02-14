namespace OCompiler.Analyze.Lexical.Tokens.BooleanLiterals
{
    internal class True : BooleanLiteral
    {
        new public static string Literal => "true";

        public True() : base(Literal) { }
    }
}
