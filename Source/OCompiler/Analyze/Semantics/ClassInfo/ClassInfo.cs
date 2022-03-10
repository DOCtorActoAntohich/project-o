using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.ClassInfo;

internal abstract class ClassInfo
{
    public abstract object Class { get; }
    public string Name { get; protected set; } = "";

    public abstract string? GetMethodReturnType(string name, List<string> argumentTypes);
    public abstract bool HasField(string name);
    public abstract bool HasConstructor(List<string> argumentTypes);
}
