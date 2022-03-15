using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;

namespace OCompiler.StandardLibrary.CodeDom;

public static class Base
{
    public const string Namespace = "OLang";
    
    public static CodeTypeDeclaration GenerateWithDefaultToString(string newClassName)
    {
        var newType = new CodeTypeDeclaration(newClassName);

        var emptyToStringMethod = new CodeMemberMethod
        {
            Name = "ToString",
            ReturnType = new CodeTypeReference(DomString.FullTypeName)
        };
        emptyToStringMethod.Statements.Add(
            new CodeMethodReturnStatement(
                new CodeObjectCreateExpression(DomString.FullTypeName)));
        
        newType.Members.Add(emptyToStringMethod);
        
        return newType;
    }
}