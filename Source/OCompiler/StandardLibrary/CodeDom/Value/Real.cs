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
        const string toStringMethodName = "ToString";
        
        var @string = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToString");
        var returnValue = new CodeObjectCreateExpression(DomString.FullTypeName, @string);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = Base.EmptyPublicMethod(DomString.FullTypeName, toStringMethodName);
        toStringMethod.Statements.Add(returnStatement);

        realType.Members.Add(toStringMethod);
    }

    private static void AddToIntegerMethod(this CodeTypeDeclaration realType)
    {
        const string toIntegerMethodName = "ToInteger";
        
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @int = new CodeMethodInvokeExpression(
            convertType, "ToInt32", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomInt.FullTypeName, @int);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toIntMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, toIntegerMethodName);
        toIntMethod.Statements.Add(returnStatement);

        realType.Members.Add(toIntMethod);
    }

    private static void AddToBooleanMethod(this CodeTypeDeclaration realType)
    {
        const string toBooleanMethodName = "ToBoolean";
        
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @bool = new CodeMethodInvokeExpression(
            convertType, "ToBoolean", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, @bool);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toBoolMethod = Base.EmptyPublicMethod(DomBool.FullTypeName, toBooleanMethodName);
        toBoolMethod.Statements.Add(returnStatement);

        realType.Members.Add(toBoolMethod);
    }

    private static void AddNegationMethod(this CodeTypeDeclaration realType)
    {
        const string negationMethodName = "Negative";
        const string rhsOperandName = "number";

        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodePrimitiveExpression(-1);
        var newValue = new CodeBinaryOperatorExpression(lhs, CodeBinaryOperatorType.Multiply, rhs);

        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var negativeMethod = Base.EmptyPublicMethod(FullTypeName, negationMethodName);
        negativeMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        negativeMethod.Statements.Add(returnStatement);

        realType.Members.Add(negativeMethod);
    }

    private static void AddMaxMethod(this CodeTypeDeclaration realType)
    {
        const string maxMethodName = "Max";
        
        var @double = new CodeTypeReferenceExpression(typeof(double));
        var doubleMax = new CodeFieldReferenceExpression(@double, "MaxValue");
        var returnValue = new CodeObjectCreateExpression(FullTypeName, doubleMax);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var maxMethod = Base.EmptyPublicMethod(FullTypeName, maxMethodName);
        maxMethod.Statements.Add(returnStatement);

        realType.Members.Add(maxMethod);
    }

    private static void AddMinMethod(this CodeTypeDeclaration realType)
    {
        const string minMethodName = "Min";
        
        var @double = new CodeTypeReferenceExpression(typeof(double));
        var doubleMin = new CodeFieldReferenceExpression(@double, "MinValue");
        var returnValue = new CodeObjectCreateExpression(FullTypeName, doubleMin);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var minMethod = Base.EmptyPublicMethod(FullTypeName, minMethodName);
        minMethod.Statements.Add(returnStatement);

        realType.Members.Add(minMethod);
    }

    private static void AddEpsilonMethod(this CodeTypeDeclaration realType)
    {
        const string epsilonMethodName = "Epsilon";
        
        var @double = new CodeTypeReferenceExpression(typeof(double));
        var epsilon = new CodeFieldReferenceExpression(@double, "Epsilon");
        var returnValue = new CodeObjectCreateExpression(FullTypeName, epsilon);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var epsilonMethod = Base.EmptyPublicMethod(FullTypeName, epsilonMethodName);
        epsilonMethod.Statements.Add(returnStatement);
        
        realType.Members.Add(epsilonMethod);
    }
}