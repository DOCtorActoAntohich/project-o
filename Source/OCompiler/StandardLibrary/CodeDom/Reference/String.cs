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
        
        stringType.AddToStringMethod();
        stringType.AddToIntegerMethod();
        stringType.AddToRealMethod();
        stringType.AddToBooleanMethod();
        
        stringType.AddAtMethod();
        stringType.AddConcatenateMethod();

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

    private static CodeMemberMethod EmptyPublicMethod(string returnType, string name)
    {
        return new CodeMemberMethod()
        {
            Name = name,
            ReturnType = new CodeTypeReference(returnType),
            Attributes = MemberAttributes.Public
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


    private static void AddToStringMethod(this CodeTypeDeclaration stringType)
    {
        var returnValue = new CodeObjectCreateExpression(FullTypeName, ReferenceInternalValue());
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = EmptyPublicMethod(FullTypeName, "ToString");
        toStringMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toStringMethod);
    }

    private static CodeMethodInvokeExpression ParseInternalValueInType(System.Type type)
    {
        var typeRef = new CodeTypeReferenceExpression(type);
        return new CodeMethodInvokeExpression(
            typeRef, "Parse", ReferenceInternalValue());
    }
    private static void AddToIntegerMethod(this CodeTypeDeclaration stringType)
    {
        var parseInt = ParseInternalValueInType(typeof(int));
        var returnValue = new CodeObjectCreateExpression(DomInt.FullTypeName, parseInt);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toIntegerMethod = EmptyPublicMethod(DomInt.FullTypeName, "ToInteger");
        toIntegerMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toIntegerMethod);
    }
    
    private static void AddToRealMethod(this CodeTypeDeclaration stringType)
    {
        var parseFloat = ParseInternalValueInType(typeof(float));
        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, parseFloat);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toRealMethod = EmptyPublicMethod(DomReal.FullTypeName, "ToReal");
        toRealMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toRealMethod);
    }
    
    private static void AddToBooleanMethod(this CodeTypeDeclaration stringType)
    {
        var parseBool = ParseInternalValueInType(typeof(bool));
        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, parseBool);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toBooleanMethod = EmptyPublicMethod(DomBool.FullTypeName, "ToBoolean");
        toBooleanMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toBooleanMethod);
    }
    
    private static void AddAtMethod(this CodeTypeDeclaration stringType)
    {
        // TODO implement this method.
    }
    
    private static void AddConcatenateMethod(this CodeTypeDeclaration stringType)
    {
        // TODO implement this method.
    }
}