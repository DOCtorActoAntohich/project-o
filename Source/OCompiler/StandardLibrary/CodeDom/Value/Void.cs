using System.CodeDom;
using DomClass = OCompiler.StandardLibrary.CodeDom.Reference.Class;

namespace OCompiler.StandardLibrary.CodeDom.Value;

internal static class Void
{
    public const string TypeName = "Void";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var voidType = Base.GenerateWithDefaultToString(TypeName);

        voidType.IsClass     = false;
        voidType.IsEnum      = false;
        voidType.IsInterface = false;
        
        voidType.IsStruct = true;

        return voidType;
    }
}