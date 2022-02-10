namespace OCompiler.Analyze.Lexical.Tokens.CommentDelimiters
{
    internal class BlockStart : CommentDelimiter
    {
        new public static string Literal => "/*";

        public BlockStart(long startOffset) : base(startOffset, Literal) { }

        static BlockStart() => ReservedTokens.RegisterToken(Literal, (pos) => new BlockStart(pos));
    }
}
