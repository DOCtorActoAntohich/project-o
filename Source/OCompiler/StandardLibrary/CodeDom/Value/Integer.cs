using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomInt = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomBool = OCompiler.StandardLibrary.CodeDom.Value.Boolean;

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

        integerType.AddInternalConstructor();
        integerType.AddDefaultConstructor();

        integerType.AddToStringMethod();
        integerType.AddToRealMethod();
        integerType.AddToBooleanMethod();
        integerType.AddNegationMethod();

        integerType.AddMaxMethod();
        integerType.AddMinMethod();

        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.Add, "Plus");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.Subtract, "Minus");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.Multiply, "Mult");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.Divide, "Div");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.Modulus, "Mod");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.LessThan, "Less");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.LessThanOrEqual, "LessEqual");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.GreaterThan, "Greater");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.GreaterThanOrEqual, "GreaterEqual");
        integerType.AddIntegerOperatorMethod(CodeBinaryOperatorType.ValueEquality, "Equal");

        return integerType;
    }

    private static void AddInternalConstructor(this CodeTypeDeclaration integerType)
    {
        var ctor = Base.EmptyPublicConstructor();

        const string paramName = "number";
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), paramName));

        var newValue = new CodeArgumentReferenceExpression(paramName);
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        integerType.Members.Add(ctor);
    }

    private static void AddDefaultConstructor(this CodeTypeDeclaration integerType)
    {
        var ctor = Base.EmptyPublicConstructor();
        var newValue = new CodePrimitiveExpression(0);
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        integerType.Members.Add(ctor);
    }

    private static void AddIntegerOperatorMethod(
    this CodeTypeDeclaration integerType,
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

        integerType.Members.Add(operatorMethod);
    }

    private static void AddToStringMethod(this CodeTypeDeclaration integerType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToString");
        var returnValue = new CodeObjectCreateExpression(DomString.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = Base.EmptyPublicMethod(DomString.FullTypeName, "ToString");
        toStringMethod.Statements.Add(returnStatement);

        integerType.Members.Add(toStringMethod);
    }

    private static void AddToRealMethod(this CodeTypeDeclaration integerType)
    {
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @double = new CodeMethodInvokeExpression(
            convertType, "ToDouble", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomReal.FullTypeName, @double);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toRealMethod = Base.EmptyPublicMethod(DomReal.FullTypeName, "ToReal");
        toRealMethod.Statements.Add(returnStatement);

        integerType.Members.Add(toRealMethod);
    }

    private static void AddToBooleanMethod(this CodeTypeDeclaration integerType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToBoolean");
        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toBooleanMethod = Base.EmptyPublicMethod(DomBool.FullTypeName, "ToBoolean");
        toBooleanMethod.Statements.Add(returnStatement);

        integerType.Members.Add(toBooleanMethod);
    }

    private static void AddNegationMethod(this CodeTypeDeclaration integerType)
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

        integerType.Members.Add(negativeMethod);
    }

    private static void AddMaxMethod(this CodeTypeDeclaration integerType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "Max");
        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var maxMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, "MaxValue");
        maxMethod.Statements.Add(returnStatement);

        integerType.Members.Add(maxMethod);
    }

    private static void AddMinMethod(this CodeTypeDeclaration integerType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "Min");
        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var minMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, "MinValue");
        minMethod.Statements.Add(returnStatement);

        integerType.Members.Add(minMethod);
    }

}