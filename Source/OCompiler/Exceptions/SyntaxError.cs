using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions
{
    internal class SyntaxError : AnalyzeError
    {
        public SyntaxError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}
