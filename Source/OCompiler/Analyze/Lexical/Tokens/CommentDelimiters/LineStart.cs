namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class LineStart : CommentDelimiter
    {
        new public static string Literal => "//";

        public LineStart(long startOffset) : base(startOffset, Literal) { }

        static LineStart() => ReservedTokens.RegisterToken(Literal, (pos) => new LineStart(pos));
    }
}
