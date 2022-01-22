namespace OCompiler.Tokens
{
    class Token
    {
        public readonly int StartOffset;
        public readonly int EndOffset;

        public readonly string Literal;

        public readonly CodeEntity Entity;
        public readonly TokenType  Type;

        public Token(int startOffset, int endOffset, string literal, TokenType type, CodeEntity entity = null)
        {
            Entity = entity ?? CodeEntity.Empty;

            StartOffset = startOffset;
            EndOffset = endOffset;
            Literal = literal;
            Type = type;
        }
    }
}
