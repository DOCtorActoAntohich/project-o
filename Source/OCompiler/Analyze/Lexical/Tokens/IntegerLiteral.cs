using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class IntegerLiteral : Token
    {
        public int Value { get; }
        public IntegerLiteral(string literal) : base(literal)
        {
            if (!literal.TryCastToInteger(out int result))
            {
                throw new System.ArgumentException("The literal specified cannot be cast to Integer type");
            }
            Value = result;
        }
    }
}
