using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomVoid   = OCompiler.StandardLibrary.CodeDom.Value.Void;

namespace OCompiler.StandardLibrary.CodeDom;

internal static class Base
{
    public const string Namespace = "OLang";
    
    public const string InternalValueVariableName = "Value";
    
    
    public static CodeTypeDeclaration GenerateWithDefaultToString(string newClassName)
    {
        var newType = NewPublicTypeDeclaration(newClassName);

        var emptyToStringMethod = Base.EmptyPublicMethod(DomString.FullTypeName, "ToString");

        emptyToStringMethod.Statements.Add(
            new CodeMethodReturnStatement(
                new CodeObjectCreateExpression(DomString.FullTypeName)));
        
        newType.Members.Add(emptyToStringMethod);
        
        return newType;
    }

    public static CodeTypeDeclaration NewPublicTypeDeclaration(string newClassName)
    {
        return new CodeTypeDeclaration(newClassName)
        {
            Attributes = MemberAttributes.Public
        };
    }


    public static CodeMemberField CreateInternalValue(System.Type type)
    {
        return new CodeMemberField(type, InternalValueVariableName)
        {
            Attributes = MemberAttributes.Public
        };
    }
    
    public static CodeFieldReferenceExpression ReferenceInternalValue()
    {
        return new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), InternalValueVariableName);
    }

    public static CodeAssignStatement WriteToInternalValue(CodeExpression newValue)
    {
        return new CodeAssignStatement(ReferenceInternalValue(), newValue);
    }

    public static CodeConstructor EmptyPublicConstructor()
    {
        return new CodeConstructor
        {
            Attributes = MemberAttributes.Public,
        };
    }

    public static CodeMemberMethod EmptyPublicMethod(string returnType, string name)
    {
        return new CodeMemberMethod
        {
            Name = name,
            ReturnType = new CodeTypeReference(returnType),
            Attributes = MemberAttributes.Public | MemberAttributes.Final
        };
    }

    public static CodeMethodReturnStatement ReturnVoid()
    {
        var voidTypeObject = new CodeObjectCreateExpression(DomVoid.FullTypeName); 
        return new CodeMethodReturnStatement(voidTypeObject);
    }
}