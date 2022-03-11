using System.Collections.Generic;
using System.Text;

namespace OCompiler.Analyze.Semantics.Class;

internal abstract class ClassInfo
{
    public abstract object? Class { get; }
    public abstract object? BaseClass { get; }
    public string Name { get; protected set; } = "";

    public abstract string? GetMethodReturnType(string name, List<string> argumentTypes);
    public abstract bool HasField(string name);
    public abstract bool HasConstructor(List<string> argumentTypes);

    public override string ToString()
    {
        StringBuilder @string = new();
        @string.Append("Unknown class [");
        @string.Append(Class);
        @string.Append(']');
        return @string.ToString();
    }
}
