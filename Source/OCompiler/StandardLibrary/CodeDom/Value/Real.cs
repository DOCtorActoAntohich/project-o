using System.CodeDom;

namespace OCompiler.StandardLibrary.CodeDom.Value;

public class Real
{
    public const string TypeName = "Real";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    
    public static CodeTypeDeclaration Generate()
    {
        var realType = Base.NewPublicTypeDeclaration(TypeName);
        
        realType.Members.Add(Base.CreateInternalValue(typeof(double)));

        return realType;
    }
}