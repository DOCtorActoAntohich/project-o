namespace OCompiler.Analyze.Lexical.Tokens.BooleanLiterals
{
    internal class False : BooleanLiteral
    {
        new public static string Literal => "false";

        public False() : base(Literal) { }
    }
}
