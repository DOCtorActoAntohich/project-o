using System;

namespace OCompiler.Exceptions
{
    internal class CompilerInternalError : Exception
    {
        public CompilerInternalError(string message) : base(message) { }
    }
}
