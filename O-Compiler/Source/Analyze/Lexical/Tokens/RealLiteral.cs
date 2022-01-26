using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class RealLiteral : Token
    {
        public double Value { get; }
        public RealLiteral(long startOffset, string literal) : base(startOffset, literal)
        {
            if (literal.ToDouble(out double result))
            {
                Value = result;
            }
            else
            {
                throw new System.ArgumentException("The literal specified cannot be cast to Real type");
            }
        }
    }
}
