using System.CodeDom;
using DomClass = OCompiler.StandardLibrary.CodeDom.Reference.Class;

namespace OCompiler.StandardLibrary.CodeDom.Value;

internal static class AnyValue
{
    public const string TypeName = "AnyValue";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var anyValueType = Base.GenerateWithDefaultToString(TypeName);
        anyValueType.BaseTypes.Add(new CodeTypeReference(DomClass.TypeName));
        
        anyValueType.IsClass     = false;
        anyValueType.IsEnum      = false;
        anyValueType.IsInterface = false;
        
        anyValueType.IsStruct = true;

        return anyValueType;
    }
}