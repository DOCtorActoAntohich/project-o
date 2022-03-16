using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions.Semantic
{
    internal class UnknownNameError : AnalyzeError
    {
        public UnknownNameError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}
