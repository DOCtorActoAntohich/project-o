using System.CodeDom;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

internal static class IO
{
    public const string TypeName = "IO";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var ioType = Base.GenerateWithDefaultToString(FullTypeName);
        ioType.BaseTypes.Add(new CodeTypeReference(DomAnyRef.FullTypeName));
        
        ioType.AddWriteMethod();
        ioType.AddWriteLineMethod();
        ioType.AddReadLineMethod();

        return ioType;
    }

    private static void AddWriteMethod(this CodeTypeDeclaration ioType)
    {
        var writeMethod = new CodeMemberMethod
        {
            Name = "Write",
            ReturnType = new CodeTypeReference(typeof(void)),
            Attributes = MemberAttributes.Public
        };
        
        const string argumentString = "str";
        writeMethod.Parameters.Add(
            new CodeParameterDeclarationExpression(
                DomString.FullTypeName, argumentString));
        
        var systemConsole = new CodeTypeReferenceExpression(typeof(System.Console));
        writeMethod.Statements.Add(new CodeMethodInvokeExpression(
            systemConsole, "Write",
            new CodeArgumentReferenceExpression(argumentString)));

        ioType.Members.Add(writeMethod);
    }
    
    
    private static void AddWriteLineMethod(this CodeTypeDeclaration ioType)
    {
        var writeLineMethod = new CodeMemberMethod
        {
            Name = "WriteLine",
            ReturnType = new CodeTypeReference(typeof(void)),
            Attributes = MemberAttributes.Public
        };
        
        const string argumentString = "str";
        writeLineMethod.Parameters.Add(
            new CodeParameterDeclarationExpression(
                DomString.FullTypeName, argumentString));
        
        var systemConsole = new CodeTypeReferenceExpression(typeof(System.Console));
        writeLineMethod.Statements.Add(new CodeMethodInvokeExpression(
            systemConsole, "WriteLine",
            new CodeArgumentReferenceExpression(argumentString)));

        ioType.Members.Add(writeLineMethod);
    }

    private static void AddReadLineMethod(this CodeTypeDeclaration ioType)
    {
        var readLineMethod = new CodeMemberMethod()
        {
            Name = "ReadLine",
            ReturnType = new CodeTypeReference(DomString.FullTypeName),
            Attributes = MemberAttributes.Public
        };

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