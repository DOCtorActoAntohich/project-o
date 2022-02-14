namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class BlockStart : CommentDelimiter
    {
        new public static string Literal => "/*";

        public BlockStart() : base(Literal) { }
    }
}
