namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class EndOfFile : Token
    {
        public EndOfFile(TokenPosition position) : base("")
        {
            Position = position;
        }
    }
}
