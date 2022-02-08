namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Delimiter : Token
    {
        protected Delimiter(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
