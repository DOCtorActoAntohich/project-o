using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions
{
    internal class TypeError : AnalyzeError
    {
        public TypeError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}
