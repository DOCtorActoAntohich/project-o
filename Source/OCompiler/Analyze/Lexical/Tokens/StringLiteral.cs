using OCompiler.Analyze.Lexical.Literals;

namespace OCompiler.Analyze.Lexical.Tokens
{
    internal class StringLiteral : Token
    {
        public string EscapedLiteral {
            get => Literal.Replace(
                Literals.Delimiter.StringQuote.Value,
                Literals.Delimiter.StringQuoteEscape.Value
            );
        }
        public StringLiteral(long startOffset, string literal) : base(startOffset, literal) { }
    }
}
