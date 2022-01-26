namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Whitespace : Token
    {
        public Whitespace(long startOffset, string literal) : base(startOffset, literal)
        {
            if (!string.IsNullOrWhiteSpace(literal))
            {
                throw new System.ArgumentException("The literal specified contains non-whitespace symbols");
            }
        }
    }
}
