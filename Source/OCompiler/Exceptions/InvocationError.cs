using System;
namespace OCompiler.Exceptions;

internal class InvocationError : Exception
{
    public InvocationError(string message) : base(message) { }
}
