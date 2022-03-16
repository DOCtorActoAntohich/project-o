using System.Collections.Generic;
using System.Text;

namespace OCompiler.Analyze.Semantics.Class;

internal abstract class ClassInfo
{
    public abstract object? Class { get; }
    public ClassInfo? BaseClass { get; protected set; }
    public List<ClassInfo> DerivedClasses { get; } = new();
    public string Name { get; protected set; } = "";

    public abstract string? GetMethodReturnType(string name, List<string> argumentTypes);
    public abstract object? GetConstructor(List<string> argumentTypes);
    public abstract string? GetFieldType(string name);
    public abstract bool HasField(string name);
    public abstract bool HasConstructor(List<string> argumentTypes);
    public abstract string ToString(bool includeBase);

    public override string ToString()
    {
        return ToString(true);
    }
}
