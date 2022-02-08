namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class CommentDelimiter : Token
    {
        public CommentDelimiter(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
