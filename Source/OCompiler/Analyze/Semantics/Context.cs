using System;
using System.Collections.Generic;

using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Analyze.Semantics;

internal class Context
{
    public ParsedClassInfo CurrentClass { get; }
    public Dictionary<string, ClassInfo>? Classes { get; private set; }
    public CallableInfo? CurrentMethod { get; }

    public Context(ParsedClassInfo currentClass, Dictionary<string, ClassInfo>? classes = null, CallableInfo? currentMethod = null)
    {
        CurrentClass = currentClass;
        CurrentMethod = currentMethod;
        if (classes != null)
        {
            Classes = classes;
        }
    }

    public void AddClasses(Dictionary<string, ClassInfo> classes)
    {
        Classes = classes;
    }

    public ClassInfo GetClassByName(string name)
    {
        if (Classes!.TryGetValue(name, out var classInfo))
        {
            return classInfo;
        }
        throw new Exception($"Unknown type: {name}");
    }
}
