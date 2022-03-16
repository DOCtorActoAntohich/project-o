using System;
using System.Collections.Generic;

using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Analyze.Semantics;

internal class Context
{
    public ParsedClassInfo CurrentClass { get; }
    public ClassTree? Classes { get; private set; }
    public CallableInfo? CurrentMethod { get; }

    public Context(ParsedClassInfo currentClass, ClassTree? classes = null, CallableInfo? currentMethod = null)
    {
        CurrentClass = currentClass;
        CurrentMethod = currentMethod;
        if (classes != null)
        {
            Classes = classes;
        }
    }

    public void AddClasses(ClassTree classes)
    {
        Classes = classes;
    }

    public ClassInfo GetClassByName(string name)
    {
        var classInfo = Classes![name];
        if (classInfo == null)
        {
            throw new Exception($"Unknown type: {name}");
        }
        return classInfo;
    }
}
