namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Keyword : Token
    {
        protected Keyword(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
