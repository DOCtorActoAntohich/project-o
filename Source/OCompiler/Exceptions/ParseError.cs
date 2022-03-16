using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions
{
    internal class ParseError : AnalyzeError
    {
        public ParseError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}
