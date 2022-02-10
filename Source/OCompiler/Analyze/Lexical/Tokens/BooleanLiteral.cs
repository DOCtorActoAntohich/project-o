namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class BooleanLiteral : Token
    {
        protected BooleanLiteral(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
