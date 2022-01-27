using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class IntegerLiteral : Token
    {
        public int Value { get; }
        public IntegerLiteral(long startOffset, string literal) : base(startOffset, literal)
        {
            if (!literal.ToInteger(out int result))
            {
                throw new System.ArgumentException("The literal specified cannot be cast to Integer type");
            }
            Value = result;
        }
    }
}
