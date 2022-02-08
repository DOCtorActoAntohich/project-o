
namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Identifier : Token
    {
        public Identifier(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
