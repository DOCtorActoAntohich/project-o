using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class BooleanLiteral : Token
    {
        public BooleanLiteral(long startOffset, string literal) : base(startOffset, literal)
        {
            if (ReservedLiteral.GetByValue(literal) is not Literals.Boolean)
            {
                throw new System.ArgumentException("The literal specified is not a valid Boolean value");
            }
        }
    }
}
