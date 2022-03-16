﻿using OCompiler.Analyze.Lexical;

namespace OCompiler.Exceptions.Semantic
{
    internal class NameCollisionError : AnalyzeError
    {
        public NameCollisionError(TokenPosition position, string message) : base($"{position}: {message}") { }
    }
}
