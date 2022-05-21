using System.Collections.Generic;
using System.Reflection;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class TypeReference : CodeObject, ICanHaveGenericTypes
{
    // true if this is a generic type like T/K/V, false if concrete name.
    public bool IsGeneric { get; set; }
    
    public List<TypeReference> GenericTypes { get; } = new();

    public MemberInfo? DotnetType;
    
    public TypeReference(string name, bool isGeneric = false, MemberInfo? dotnetType = null) : base(name)
    {
        IsGeneric = isGeneric;
        DotnetType = dotnetType;
    }
}