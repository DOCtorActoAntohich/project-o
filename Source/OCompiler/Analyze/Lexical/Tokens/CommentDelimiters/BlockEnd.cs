namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class BlockEnd : CommentDelimiter
    {
        new public static string Literal => "*/";

        public BlockEnd() : base(Literal) { }
    }
}
