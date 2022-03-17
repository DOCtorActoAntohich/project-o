using System.CodeDom;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomInt  = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomBool = OCompiler.StandardLibrary.CodeDom.Value.Boolean;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

internal static class String
{
    public const string TypeName = "String";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";

    public static CodeTypeDeclaration Generate()
    {
        var stringType = Base.NewPublicTypeDeclaration(TypeName);
        stringType.BaseTypes.Add(new CodeTypeReference(DomAnyRef.TypeName));

        stringType.Members.Add(Base.CreateInternalValue(typeof(string)));
        
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


    private static void AddInternalConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "str";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), paramName));
        
        var newValue = new CodeArgumentReferenceExpression(paramName);
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        
        stringType.Members.Add(ctor);
    }
    
    private static void AddDefaultConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();
        
        var newValue = new CodePrimitiveExpression("");
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        stringType.Members.Add(ctor);
    }
    
    private static void AddAnyRefConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "obj";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomAnyRef.FullTypeName, paramName));

        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        
        stringType.Members.Add(ctor);
    }
    
    private static void AddIntegerConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomInt.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }
    
    private static void AddRealConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomReal.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }
    
    private static void AddBooleanConstructor(this CodeTypeDeclaration stringType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(DomBool.FullTypeName, paramName));
        
        var newValue = new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression(paramName), "ToString");
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));


        stringType.Members.Add(ctor);
    }


    private static void AddToStringMethod(this CodeTypeDeclaration stringType)
    {
        var returnValue = new CodeObjectCreateExpression(FullTypeName, Base.ReferenceInternalValue());
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = Base.EmptyPublicMethod(FullTypeName, "ToString");
        toStringMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toStringMethod);
    }

    private static CodeMethodInvokeExpression ParseInternalValueInType(System.Type type)
    {
        var typeRef = new CodeTypeReferenceExpression(type);
        return new CodeMethodInvokeExpression(
            typeRef, "Parse", Base.ReferenceInternalValue());
    }
    private static void AddToIntegerMethod(this CodeTypeDeclaration stringType)
    {
        var parseInt = ParseInternalValueInType(typeof(int));
        var returnValue = new CodeObjectCreateExpression(DomInt.FullTypeName, parseInt);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toIntegerMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, "ToInteger");
        toIntegerMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toIntegerMethod);
    }
    
    private static void AddToRealMethod(this CodeTypeDeclaration stringType)
    {
        var parseFloat = ParseInternalValueInType(typeof(float));
        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, parseFloat);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toRealMethod = Base.EmptyPublicMethod(DomReal.FullTypeName, "ToReal");
        toRealMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toRealMethod);
    }
    
    private static void AddToBooleanMethod(this CodeTypeDeclaration stringType)
    {
        var parseBool = ParseInternalValueInType(typeof(bool));
        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, parseBool);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var toBooleanMethod = Base.EmptyPublicMethod(DomBool.FullTypeName, "ToBoolean");
        toBooleanMethod.Statements.Add(returnStatement);

        stringType.Members.Add(toBooleanMethod);
    }
    
    private static void AddAtMethod(this CodeTypeDeclaration stringType)
    {
        const string atMethodName = "At";
        const string paramName = "index";

        var index = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        var one = new CodePrimitiveExpression(1);
        var nextIndex = new CodeBinaryOperatorExpression(index, CodeBinaryOperatorType.Add, one);

        var @string = Base.ReferenceInternalValue();
        var substr = new CodeMethodInvokeExpression(
            @string, "Substring", index, nextIndex);
        
        var returnValue = new CodeObjectCreateExpression(FullTypeName, substr);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var atMethod = Base.EmptyPublicMethod(FullTypeName, atMethodName);
        atMethod.Parameters.Add(new CodeParameterDeclarationExpression(DomInt.FullTypeName, paramName));

        atMethod.Statements.Add(returnStatement);

        stringType.Members.Add(atMethod);
    }
    
    private static void AddConcatenateMethod(this CodeTypeDeclaration stringType)
    {
        const string concatenateMethodName = "Concatenate";
        const string paramName = "other";

        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        var concatenatedStrings = new CodeBinaryOperatorExpression(lhs, CodeBinaryOperatorType.Add, rhs);

        var returnValue = new CodeObjectCreateExpression(FullTypeName, concatenatedStrings);
        var returnStatement = new CodeMethodReturnStatement(returnValue);
        
        var concatenateMethod = Base.EmptyPublicMethod(FullTypeName, concatenateMethodName);
        concatenateMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, paramName));

        concatenateMethod.Statements.Add(returnStatement);

        stringType.Members.Add(concatenateMethod);
    }
}