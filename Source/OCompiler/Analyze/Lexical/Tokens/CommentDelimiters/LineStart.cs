namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class LineStart : CommentDelimiter
    {
        new public static string Literal => "//";

        public LineStart() : base(Literal) { }
    }
}
