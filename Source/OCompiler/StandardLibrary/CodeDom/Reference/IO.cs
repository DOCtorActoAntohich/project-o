using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomVoid   = OCompiler.StandardLibrary.CodeDom.Value.Void;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

internal static class IO
{
    public const string TypeName = "IO";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var ioType = Base.GenerateWithDefaultToString(TypeName);
        ioType.BaseTypes.Add(new CodeTypeReference(DomAnyRef.FullTypeName));
        
        ioType.AddWriteMethod();
        ioType.AddWriteLineMethod();
        ioType.AddReadLineMethod();

        return ioType;
    }

    private static void AddWriteMethod(this CodeTypeDeclaration ioType)
    {
        var writeMethod = Base.EmptyPublicMethod(DomVoid.FullTypeName, "Write");

        const string paramName = "str";
        writeMethod.Parameters.Add(new CodeParameterDeclarationExpression(DomString.FullTypeName, paramName));
        
        var systemConsole = new CodeTypeReferenceExpression(typeof(System.Console));
        var @string = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        var printingStatement = new CodeMethodInvokeExpression(systemConsole, "Write", @string);
        
        writeMethod.Statements.Add(printingStatement);
        writeMethod.Statements.Add(Base.ReturnVoid());

        ioType.Members.Add(writeMethod);
    }
    
    
    private static void AddWriteLineMethod(this CodeTypeDeclaration ioType)
    {
        var writeLineMethod = Base.EmptyPublicMethod(DomVoid.FullTypeName, "WriteLine");

        const string paramName = "str";
        writeLineMethod.Parameters.Add(new CodeParameterDeclarationExpression(DomString.FullTypeName, paramName));
        
        var systemConsole = new CodeTypeReferenceExpression(typeof(System.Console));
        var @string = new CodeFieldReferenceExpression(
            new CodeArgumentReferenceExpression(paramName), Base.InternalValueVariableName);
        var printingStatement = new CodeMethodInvokeExpression(systemConsole, "WriteLine", @string);

        writeLineMethod.Statements.Add(printingStatement);
        writeLineMethod.Statements.Add(Base.ReturnVoid());

        ioType.Members.Add(writeLineMethod);
    }

    private static void AddReadLineMethod(this CodeTypeDeclaration ioType)
    {
        var readLineMethod = Base.EmptyPublicMethod(DomString.FullTypeName, "ReadLine");

        const string resultVar = "result";

        var systemConsole = new CodeTypeReferenceExpression(typeof(System.Console));
        var declareResultVar = new CodeVariableDeclarationStatement("string?", resultVar, 
            new CodeMethodInvokeExpression(
                systemConsole, "ReadLine"));

        
        var isResultNull = new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression(resultVar), 
            CodeBinaryOperatorType.IdentityEquality,
            new CodePrimitiveExpression(null));
        var isResultNullWhenTrue = new CodeMethodReturnStatement(
            new CodeObjectCreateExpression(DomString.FullTypeName));
        var nullCheckBlock = new CodeConditionStatement(isResultNull, isResultNullWhenTrue);


        var defaultReturn = new CodeMethodReturnStatement(
            new CodeObjectCreateExpression(
                DomString.FullTypeName, 
                new CodeVariableReferenceExpression(resultVar)));

        
        readLineMethod.Statements.Add(declareResultVar);
        readLineMethod.Statements.Add(nullCheckBlock);
        readLineMethod.Statements.Add(defaultReturn);

        ioType.Members.Add(readLineMethod);
    }
}