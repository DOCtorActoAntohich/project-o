using OCompiler.Utils;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class RealLiteral : Token
    {
        public double Value { get; }
        public RealLiteral(string literal) : base(literal)
        {
            if (!literal.TryCastToDouble(out double result))
            {
                throw new System.ArgumentException("The literal specified cannot be cast to Real type");
            }
            Value = result;
        }
    }
}
