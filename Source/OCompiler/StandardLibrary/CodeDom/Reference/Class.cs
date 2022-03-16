using System.CodeDom;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

public static class Class
{
    public const string TypeName = "Class";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var classType = Base.GenerateWithDefaultToString(TypeName);
        classType.BaseTypes.Add(typeof(object));

        return classType;
    }
}