using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class CommentDelimiter : Token
    {
        public bool IsBlockCommentStart { get => Literal == Literals.CommentDelimiter.BlockStart.Value; }
        public bool IsBlockCommentEnd { get => Literal == Literals.CommentDelimiter.BlockEnd.Value; }
        public bool IsLineCommentStart { get => Literal == Literals.CommentDelimiter.LineStart.Value; }
        public CommentDelimiter(long startOffset, string literal) : base(startOffset, literal)
        {
            if (ReservedLiteral.GetByValue(literal) is not Literals.CommentDelimiter)
            {
                throw new System.ArgumentException("The literal specified is not a valid comment delimiter");
            }
        }
    }
}
