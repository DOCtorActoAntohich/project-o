using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class TypeReference : CodeObject, ICanHaveGenericTypes
{
    // true if this is a generic type like T/K/V, false if concrete name.
    public bool IsGeneric { get; set; }
    
    public List<TypeReference> GenericTypes { get; } = new();
    public bool HasGenerics => GenericTypes.Count > 0;

    public MemberInfo? DotnetType;
    
    public TypeReference(string name, bool isGeneric = false, MemberInfo? dotnetType = null) : base(name)
    {
        IsGeneric = isGeneric;
        DotnetType = dotnetType;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(Name);

        if (GenericTypes.Count == 0)
        {
            return stringBuilder.ToString();
        }
        
        return stringBuilder
            .Append('<')
            .Append(string.Join(", ", GenericTypes))
            .Append('>')
            .ToString();
    }

    public bool SameAs(TypeReference other)
    {
        if (IsGeneric ^ other.IsGeneric)
        {
            return false;
        }

        if (Name != other.Name)
        {
            return false;
        }

        if (GenericTypes.Count != other.GenericTypes.Count)
        {
            return false;
        }

        for (var i = 0; i < GenericTypes.Count; ++i)
        {
            var type = GenericTypes[i];
            var otherType = other.GenericTypes[i];
            if (type.DifferentFrom(otherType))
            {
                return false;
            }
        }

        return true;
    }

    public bool DifferentFrom(TypeReference other)
    {
        return !SameAs(other);
    }
}