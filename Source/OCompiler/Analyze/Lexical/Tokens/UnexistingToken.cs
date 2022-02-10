namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class UnexistingToken : Token
    {
        public UnexistingToken(long position) : base(position, "") { }
    }
}
