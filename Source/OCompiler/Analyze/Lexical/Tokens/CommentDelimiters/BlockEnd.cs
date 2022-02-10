namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class BlockEnd : CommentDelimiter
    {
        new public static string Literal => "*/";

        public BlockEnd(long startOffset) : base(startOffset, Literal) { }

        static BlockEnd() => ReservedTokens.RegisterToken(Literal, (pos) => new BlockEnd(pos));
    }
}
