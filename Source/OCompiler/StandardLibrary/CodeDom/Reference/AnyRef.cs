using System.CodeDom;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

public static class AnyRef
{
    public const string TypeName = "AnyRef";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var anyRefType = Base.GenerateWithDefaultToString(TypeName);

        return anyRefType;
    }
}