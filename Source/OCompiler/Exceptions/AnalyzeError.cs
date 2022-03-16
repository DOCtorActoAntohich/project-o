using System;

using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions;

internal class AnalyzeError : Exception
{
    public AnalyzeError(string message) : base(message) { }
    public AnalyzeError(TokenPosition position, string message) : base($"{position}: {message}") { }
}
