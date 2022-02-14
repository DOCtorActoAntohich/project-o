namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class EndOfFile : Token
    {
        public EndOfFile(long position) : base("")
        {
            StartOffset = position;
        }
    }
}
