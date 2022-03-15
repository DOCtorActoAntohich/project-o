using System.CodeDom;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomInt  = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomBool = OCompiler.StandardLibrary.CodeDom.Value.Boolean;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

public static class String
{
    public const string TypeName = "String";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";

    private const string InternalValueVariable = "Value";
    
    public static CodeTypeDeclaration Generate()
    {
        var stringType = new CodeTypeDeclaration(TypeName);

        stringType.Members.Add(new CodeMemberField(typeof(string), InternalValueVariable));
        
        stringType.AddInternalConstructor();
        stringType.AddDefaultConstructor();
        stringType.AddAnyRefConstructor();
        stringType.AddIntegerConstructor();
        stringType.AddRealConstructor();
        stringType.AddBooleanConstructor();
        
        return stringType;
    }

    
    private static CodeFieldReferenceExpression ReferenceInternalValue()
    {
        return new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), InternalValueVariable);
    }

    private static CodeAssignStatement WriteToInternalValue(CodeExpression newValue)
    {
        return new CodeAssignStatement(ReferenceInternalValue(), newValue);
    }

    private static CodeConstructor EmptyPublicConstructor()
    {
        return new CodeConstructor
        {
            Attributes = MemberAttributes.Public,
        };
    }
    
    
    private static void AddInternalConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();

        const string paramName = "str";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), paramName));
        
        var newValue = new CodeArgumentReferenceExpression(paramName);
        ctor.Statements.Add(WriteToInternalValue(newValue));

        
        stringType.Members.Add(ctor);
    }
    
    private static void AddDefaultConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();
        
        var newValue = new CodePrimitiveExpression("");
        ctor.Statements.Add(WriteToInternalValue(newValue));

        stringType.Members.Add(ctor);
    }
    
    private static void AddAnyRefConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();

        const string paramName = "obj";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomAnyRef.FullTypeName, paramName));

        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(WriteToInternalValue(newValue));

        
        stringType.Members.Add(ctor);
    }
    
    private static void AddIntegerConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomInt.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }
    
    private static void AddRealConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomReal.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }
    
    private static void AddBooleanConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomBool.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }


    private static void AddToIntegerMethod(this CodeTypeDeclaration stringType)
    {
        
    }
}