namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Whitespace : Token
    {
        public Whitespace(string literal) : base(literal)
        {
            if (!string.IsNullOrWhiteSpace(literal))
            {
                throw new System.ArgumentException("The literal specified contains non-whitespace symbols");
            }
        }
    }
}
