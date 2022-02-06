namespace OCompiler.Analyze.Lexical.Literals
{
    internal class CommentDelimiter : ReservedLiteral
    {
        public static CommentDelimiter BlockStart { get; } = new("/*");
        public static CommentDelimiter BlockEnd   { get; } = new("*/");
        public static CommentDelimiter LineStart  { get; } = new("//");

        private CommentDelimiter(string literal) : base(literal) { }
    }
}
