using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Type;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    public bool IsValid(TypeReference typeReference)
    {
        if (typeReference.IsGeneric)
        {
            return true;
        }
        
        return HasClass(typeReference.Name) && typeReference.GenericTypes.All(IsValid);
    }
}