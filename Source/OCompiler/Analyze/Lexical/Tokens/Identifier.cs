using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Identifier : Token
    {
        public Identifier(long startOffset, string literal) : base(startOffset, literal)
        {
            if (ReservedLiteral.GetByValue(literal) != ReservedLiteral.Empty)
            {
                throw new System.ArgumentException("The literal specified is reserved and cannot be used as identifier");
            }
        }
    }
}
