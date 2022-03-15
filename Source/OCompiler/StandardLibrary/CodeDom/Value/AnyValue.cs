using System.CodeDom;

namespace OCompiler.StandardLibrary.CodeDom.Value;

public static class AnyValue
{
    public const string TypeName = "AnyValue";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var anyValueType = Base.GenerateWithDefaultToString(TypeName);
        
        anyValueType.IsClass     = false;
        anyValueType.IsEnum      = false;
        anyValueType.IsInterface = false;
        
        anyValueType.IsStruct = true;

        return anyValueType;
    }
}