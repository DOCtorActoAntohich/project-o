using System.CodeDom;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

public static class Time
{
    public const string TypeName = "Time";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var timeType = Base.GenerateWithDefaultToString(TypeName);
        timeType.BaseTypes.Add(new CodeTypeReference(DomAnyRef.FullTypeName));

        return timeType;
    }
}