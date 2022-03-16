using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions.Semantic
{
    internal class AccessViolationError : AnalyzeError
    {
        public AccessViolationError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}