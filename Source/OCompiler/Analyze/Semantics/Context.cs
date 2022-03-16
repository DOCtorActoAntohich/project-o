using System;

using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.Semantics;

internal class Context
{
    public ParsedClassInfo Class { get; }
    public CallableInfo? Callable { get; }

    public Context(ParsedClassInfo currentClass, CallableInfo? currentCallable = null)
    {
        Class = currentClass;
        Callable = currentCallable;
    }

    public Context WithCallable(CallableInfo callable)
    {
        // Create a copy of current context and add a callable to it.
        return new(Class, callable);
    }
}
