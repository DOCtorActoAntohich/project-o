using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal interface ICanHaveGenericTypes
{
    public List<TypeReference> GenericTypes { get; }
}