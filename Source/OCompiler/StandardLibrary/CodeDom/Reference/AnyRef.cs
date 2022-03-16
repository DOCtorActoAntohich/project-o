using System.CodeDom;
using DomClass = OCompiler.StandardLibrary.CodeDom.Reference.Class;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

public static class AnyRef
{
    public const string TypeName = "AnyRef";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var anyRefType = Base.GenerateWithDefaultToString(TypeName);
        anyRefType.BaseTypes.Add(new CodeTypeReference(DomClass.TypeName));

        return anyRefType;
    }
}