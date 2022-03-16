using System.CodeDom;

namespace OCompiler.StandardLibrary.CodeDom.Value;

public static class Integer
{
    public const string TypeName = "Integer";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";


    public static CodeTypeDeclaration Generate()
    {
        var integerType = Base.NewPublicTypeDeclaration(TypeName);
        integerType.IsStruct = true;
        integerType.IsClass = false;
        
        integerType.Members.Add(Base.CreateInternalValue(typeof(int)));

        return integerType;
    }
}