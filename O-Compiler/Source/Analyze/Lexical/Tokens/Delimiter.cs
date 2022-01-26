using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Delimiter : Token
    {
        public Delimiter(long startOffset, string literal) : base(startOffset, literal)
        {
            if (ReservedLiteral.GetByValue(literal) is not Literals.Delimiter)
            {
                throw new System.ArgumentException("The literal specified is not a valid delimiter");
            }
        }
    }
}
