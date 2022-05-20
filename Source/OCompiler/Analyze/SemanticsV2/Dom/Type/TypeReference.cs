using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class TypeReference : CodeObject, ICanHaveGenericTypes
{
    // true if this is a generic type like T/K/V, false if concrete name.
    public bool IsGeneric { get; set; }
    
    public List<TypeReference> GenericTypes { get; } = new();

    
    public TypeReference(string name, bool isGeneric = false) : base(name)
    {
        IsGeneric = isGeneric;
    }
}