using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;

namespace OCompiler.StandardLibrary.CodeDom.Value;

public static class Boolean
{
    public const string TypeName = "Boolean";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";

    public static CodeTypeDeclaration Generate()
    {
        var booleanType = Base.NewPublicTypeDeclaration(TypeName);
        
        booleanType.Members.Add(Base.CreateInternalValue(typeof(bool)));
        
        booleanType.AddInternalConstructor();
        booleanType.AddBooleanConstructor();
        
        booleanType.AddToStringMethod();
        
        booleanType.AddBinaryOperatorMethod(CodeBinaryOperatorType.BooleanAnd, "And");
        booleanType.AddBinaryOperatorMethod(CodeBinaryOperatorType.BooleanOr,  "Or");
        booleanType.AddNotOperatorMethod();
        booleanType.AddXorOperatorMethod();

        return booleanType;
    }


    private static void AddInternalConstructor(this CodeTypeDeclaration booleanType)
    {
        const string paramName = "p";
        var newValue = new CodeArgumentReferenceExpression(paramName);
        
        var ctor = Base.EmptyPublicConstructor();
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), paramName));
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        booleanType.Members.Add(ctor);
    }
    
    private static void AddBooleanConstructor(this CodeTypeDeclaration booleanType)
    {
        const string paramName = "p";

        var newValue = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        
        var ctor = Base.EmptyPublicConstructor();
        ctor.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, paramName));
        ctor.Statements.Add(Base.WriteToInternalValue(newValue));

        booleanType.Members.Add(ctor);
    }

    private static void AddToStringMethod(this CodeTypeDeclaration booleanType)
    {
        var creationParam = new CodeMethodInvokeExpression(
            Base.ReferenceInternalValue(), "ToString");
        var returnValue = new CodeObjectCreateExpression(DomString.FullTypeName, creationParam);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var toStringMethod = Base.EmptyPublicMethod(DomString.FullTypeName, "ToString");
        toStringMethod.Statements.Add(returnStatement);

        booleanType.Members.Add(toStringMethod);
    }

    private static void AddBinaryOperatorMethod(
        this CodeTypeDeclaration booleanType, 
        CodeBinaryOperatorType op, 
        string methodName)
    {
        const string rhsOperandName = "p";
        
        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(rhsOperandName), Base.InternalValueVariableName);
        var newValue = new CodeBinaryOperatorExpression(lhs, op, rhs);
        
        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var operatorMethod = Base.EmptyPublicMethod(FullTypeName, methodName);
        operatorMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        operatorMethod.Statements.Add(returnStatement);

        booleanType.Members.Add(operatorMethod);
    }

    private static void AddNotOperatorMethod(this CodeTypeDeclaration booleanType)
    {
        // !x  is equivalent to  x == false;
        var lhs = Base.ReferenceInternalValue();
        var rhs = new CodePrimitiveExpression(false);
        
        var newValue = new CodeBinaryOperatorExpression(lhs, CodeBinaryOperatorType.ValueEquality, rhs);
        var returnValue = new CodeObjectCreateExpression(FullTypeName, newValue);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var operatorMethod = Base.EmptyPublicMethod(FullTypeName, "Not");
        operatorMethod.Statements.Add(returnStatement);

        booleanType.Members.Add(operatorMethod);
    }

    private static void AddXorOperatorMethod(this CodeTypeDeclaration booleanType)
    {
        const string rhsOperandName = "p";

        // xor = (a and notB) || (notA and b);
        // In OLang: return this.And(other.Not()).Or(other.And(this.Not()));
        var a = new CodeThisReferenceExpression();
        var b = new CodeArgumentReferenceExpression(rhsOperandName);
        var notA = new CodeMethodInvokeExpression(a, "Not");
        var notB = new CodeMethodInvokeExpression(b, "Not");

        var lhs = new CodeMethodInvokeExpression(a, "And", notB);
        var rhs = new CodeMethodInvokeExpression(b, "And", notA);

        var newValue = new CodeMethodInvokeExpression(lhs, "Or", rhs);
        var returnStatement = new CodeMethodReturnStatement(newValue);

        var xorMethod = Base.EmptyPublicMethod(FullTypeName, "Xor");
        xorMethod.Parameters.Add(new CodeParameterDeclarationExpression(FullTypeName, rhsOperandName));
        xorMethod.Statements.Add(returnStatement);

        booleanType.Members.Add(xorMethod);
    }
}