using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomInt = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomBool = OCompiler.StandardLibrary.CodeDom.Value.Boolean;

namespace OCompiler.StandardLibrary.CodeDom.Value;

public static class Real
{
    public const string TypeName = "Real";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    
    public static CodeTypeDeclaration Generate()
    {
        var realType = Base.NewPublicTypeDeclaration(TypeName);
        realType.IsStruct = true;
        realType.IsClass = false;
        
        realType.Members.Add(Base.CreateInternalValue(typeof(double)));

        realType.AddInternalConstructor();
        realType.AddDefaultConstructor();

        realType.AddToStringMethod();
        realType.AddToIntegerMethod();
        realType.AddToBooleanMethod();
        realType.AddNegationMethod();

        realType.AddMaxMethod();
        realType.AddMinMethod();
        realType.AddEpsilonMethod();

        realType.AddRealOperatorMethod(CodeBinaryOperatorType.Add, "Plus");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.Subtract, "Minus");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.Multiply, "Mult");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.Divide, "Div");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.Modulus, "Mod");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.LessThan, "Less");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.LessThanOrEqual, "LessEqual");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.GreaterThan, "Greater");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.GreaterThanOrEqual, "GreaterEqual");
        realType.AddRealOperatorMethod(CodeBinaryOperatorType.ValueEquality, "Equal");

        return realType;
    }

    private static void AddInternalConstructor(this CodeTypeDeclaration realType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), paramName));

        var newValue = new CodeArgumentReferenceExpression(paramName);
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        realType.Members.Add(ctor);
    }

    private static void AddDefaultConstructor(this CodeTypeDeclaration realType)
    {
        var ctor = Base.EmptyPublicConstructor();
        var newValue = new CodePrimitiveExpression(0.0);
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        realType.Members.Add(ctor);
    }

    private static void AddRealOperatorMethod(
    this CodeTypeDeclaration realType,
    CodeBinaryOperatorType op,
    string methodName)
    {
        const string rhsOperandName = "number";

        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(rhsOperandName), Base.InternalValueVariableName);
        var newValue = new CodeBinaryOperatorExpression(lhs, op, rhs);

        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var operatorMethod = Base.EmptyPublicMethod(FullTypeName, methodName);
        operatorMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        operatorMethod.Statements.Add(returnStatement);

        realType.Members.Add(operatorMethod);
    }

    private static void AddToStringMethod(this CodeTypeDeclaration realType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToString");
        var returnValue = new CodeObjectCreateExpression(DomString.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = Base.EmptyPublicMethod(DomString.FullTypeName, "ToString");
        toStringMethod.Statements.Add(returnStatement);

        realType.Members.Add(toStringMethod);
    }

    private static void AddToIntegerMethod(this CodeTypeDeclaration realType)
    {
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @int = new CodeMethodInvokeExpression(
            convertType, "ToInt32", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomInt.FullTypeName, @int);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toIntMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, "ToInteger");
        toIntMethod.Statements.Add(returnStatement);

        realType.Members.Add(toIntMethod);
    }

    private static void AddToBooleanMethod(this CodeTypeDeclaration realType)
    {
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @boolean = new CodeMethodInvokeExpression(
            convertType, "ToBoolean", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, @boolean);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toBoolMethod = Base.EmptyPublicMethod(DomBool.FullTypeName, "ToBoolean");
        toBoolMethod.Statements.Add(returnStatement);

        realType.Members.Add(toBoolMethod);
    }

    private static void AddNegationMethod(this CodeTypeDeclaration realType)
    {
        const string rhsOperandName = "number";

        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodePrimitiveExpression(-1);
        var newValue = new CodeBinaryOperatorExpression(lhs, CodeBinaryOperatorType.Multiply, rhs);

        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var negativeMethod = Base.EmptyPublicMethod(FullTypeName, "UnaryMinus");
        negativeMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        negativeMethod.Statements.Add(returnStatement);

        realType.Members.Add(negativeMethod);
    }

    private static void AddMaxMethod(this CodeTypeDeclaration realType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "MaxValue");
        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var maxMethod = Base.EmptyPublicMethod(DomReal.FullTypeName, "Max");
        maxMethod.Statements.Add(returnStatement);

        realType.Members.Add(maxMethod);
    }

    private static void AddMinMethod(this CodeTypeDeclaration realType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "MinValue");
        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var minMethod = Base.EmptyPublicMethod(DomReal.FullTypeName, "Min");
        minMethod.Statements.Add(returnStatement);

        realType.Members.Add(minMethod);
    }

    private static void AddEpsilonMethod(this CodeTypeDeclaration realType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "Epsilon");
        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var minMethod = Base.EmptyPublicMethod(DomReal.FullTypeName, "Epsilon");
        minMethod.Statements.Add(returnStatement);

        realType.Members.Add(minMethod);
    }
}