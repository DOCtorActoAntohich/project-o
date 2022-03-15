using System;
using System.Collections.Generic;
using System.Text;

namespace OCompiler.Analyze.Semantics.Class;

internal abstract class ClassInfo
{
    public abstract object? Class { get; }
    public abstract object? BaseClass { get; }
    public string Name { get; protected init; } = "";

    public abstract string? GetMethodReturnType(string name, List<string> argumentTypes);
    public abstract string? GetFieldType(string name);
    public abstract bool HasField(string name);
    public abstract bool HasConstructor(List<string> argumentTypes);

    public bool IsValueType()
    {
        var parentClass = BaseClass;
        if (BaseClass == null)
        {
            return false;
        }

        ClassInfo? parent;
        switch (BaseClass)
        {
            case ClassInfo p:
                parent = p;
                break;
            case Type type:
                return type.IsValueType;
            default:
                return false;
        }
        
        while (parent.BaseClass != null)
        {
            switch (parent.BaseClass)
            {
                case ClassInfo info:
                    parent = info;
                    break;
                case Type t:
                    return t.IsValueType;
            }
        }

        return false;
    }
    
    public override string ToString()
    {
        StringBuilder @string = new();
        @string.Append("Unknown class [");
        @string.Append(Class);
        @string.Append(']');
        return @string.ToString();
    }
}
