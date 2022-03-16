using System;
using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions;

internal class CompilationError : Exception
{
    public CompilationError(string message) : base(message) { }
    public CompilationError(TokenPosition position, string message) : base($"{position}: {message}") { }
}

