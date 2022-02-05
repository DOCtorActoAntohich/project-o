using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class Keyword : Token
    {
        public Keyword(long startOffset, string literal) : base(startOffset, literal)
        {
            if (ReservedLiteral.GetByValue(literal) is not Literals.Keyword)
            {
                throw new System.ArgumentException("The literal specified is not a reserved keyword");
            }
        }
    }
}
