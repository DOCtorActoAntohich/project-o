using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomInt = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomBool = OCompiler.StandardLibrary.CodeDom.Value.Boolean;

namespace OCompiler.StandardLibrary.CodeDom.Value;

internal static class Integer
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
        integerType.AddIntegerConstructor();

        integerType.AddToStringMethod();
        integerType.AddToRealMethod();
        integerType.AddToBooleanMethod();
        integerType.AddNegationMethod();

        integerType.AddMaxMethod();
        integerType.AddMinMethod();

        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.Add, "Plus");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.Subtract, "Minus");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.Multiply, "Mult");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.Divide, "Div");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.Modulus, "Mod");
        
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.LessThan, "Less");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.LessThanOrEqual, "LessEqual");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.GreaterThan, "Greater");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.GreaterThanOrEqual, "GreaterEqual");
        integerType.AddBinaryOperatorMethod(CodeBinaryOperatorType.ValueEquality, "Equal");

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
    
    private static void AddIntegerConstructor(this CodeTypeDeclaration integerType)
    {
        const string paramName = "number";
        
        var newValue = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        
        var ctor = Base.EmptyPublicConstructor();
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, paramName));
        
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        integerType.Members.Add(ctor);
    }

    private static void AddBinaryOperatorMethod(
        this CodeTypeDeclaration integerType,
        CodeBinaryOperatorType op,
        string methodName)
    {
        const string rhsOperandName = "number";

        var returnType = op switch
        {
            CodeBinaryOperatorType.Add => FullTypeName,
            CodeBinaryOperatorType.Subtract => FullTypeName,
            CodeBinaryOperatorType.Multiply => FullTypeName,
            CodeBinaryOperatorType.Divide => FullTypeName,
            CodeBinaryOperatorType.Modulus => FullTypeName,
            _ => DomBool.FullTypeName
        };

        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(rhsOperandName), Base.InternalValueVariableName);
        var newValue = new CodeBinaryOperatorExpression(lhs, op, rhs);

        var returnValue = new CodeObjectCreateExpression(returnType, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var operatorMethod = Base.EmptyPublicMethod(returnType, methodName);
        operatorMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        operatorMethod.Statements.Add(returnStatement);

        integerType.Members.Add(operatorMethod);
    }

    private static void AddToStringMethod(this CodeTypeDeclaration integerType)
    {
        var intToStr = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToString");
        var returnValue = new CodeObjectCreateExpression(DomString.FullTypeName, intToStr);
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
        var convertType = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @bool = new CodeMethodInvokeExpression(
            convertType, "ToBoolean", Base.ReferenceInternalValue());

        var returnValue = new CodeObjectCreateExpression(DomBool.FullTypeName, @bool);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toBoolMethod = Base.EmptyPublicMethod(DomBool.FullTypeName, "ToBoolean");
        toBoolMethod.Statements.Add(returnStatement);

        integerType.Members.Add(toBoolMethod);
    }

    private static void AddNegationMethod(this CodeTypeDeclaration integerType)
    {
        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodePrimitiveExpression(-1);
        var newValue = new CodeBinaryOperatorExpression(lhs, CodeBinaryOperatorType.Multiply, rhs);

        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var negativeMethod = Base.EmptyPublicMethod(FullTypeName, "Negative");
        negativeMethod.Statements.Add(returnStatement);

        integerType.Members.Add(negativeMethod);
    }

    private static void AddMaxMethod(this CodeTypeDeclaration integerType)
    {
        const string maxValueFieldName = "MaxValue";
        var @int = new CodeTypeReferenceExpression(typeof(int));
        var intMax = new CodeFieldReferenceExpression(@int, maxValueFieldName);
        var returnValue = new CodeObjectCreateExpression(FullTypeName, intMax);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var maxMethod = Base.EmptyPublicMethod(FullTypeName, "Max");
        maxMethod.Statements.Add(returnStatement);

        integerType.Members.Add(maxMethod);
    }

    private static void AddMinMethod(this CodeTypeDeclaration integerType)
    {
        const string minValueFieldName = "MinValue";
        var @int = new CodeTypeReferenceExpression(typeof(int));
        var intMin = new CodeFieldReferenceExpression(@int, minValueFieldName);
        var returnValue = new CodeObjectCreateExpression(FullTypeName, intMin);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var minMethod = Base.EmptyPublicMethod(FullTypeName, "Min");
        minMethod.Statements.Add(returnStatement);

        integerType.Members.Add(minMethod);
    }
}