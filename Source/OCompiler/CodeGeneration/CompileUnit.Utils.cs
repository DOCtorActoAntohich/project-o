using System.CodeDom;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.StandardLibrary.CodeDom.Reference;
using OCompiler.StandardLibrary.CodeDom.Value;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private CodeTypeDeclaration _currentTypeDeclaration = null!;
    private CodeMemberMethod? _currentCallable;
    private ParsedClassInfo? _currentClassInfo;
    private CallableInfo? _currentCallableInfo;

    private Context CurrentContext()
    {
        return new Context(_currentClassInfo!, _currentCallableInfo);
    }

    private ExpressionInfo ExpressionInfoInCurrentContext(Expression expression)
    {
        return new ExpressionInfo(expression, CurrentContext());
    }

    
    private static bool IsClassName(string name)
    {
        var classInfo = ParsedClassInfo.GetByName(name);
        return classInfo is not EmptyParsedClassInfo;
    }
    
    private bool CurrentCallableHasParameter(string parameterName)
    {
        if (_currentCallableInfo == null)
        {
            return false;
        }
        return _currentCallableInfo.Parameters.Any(parameterInfo => parameterInfo.Name == parameterName);
    }

    
    private static bool IsPrimitiveTypeToken(Token token)
    {
        return token switch
        {
            IntegerLiteral => true,
            RealLiteral => true,
            BooleanLiteral => true,
            StringLiteral => true,
            _ => false
        };
    }
    
    private static string? ConvertPrimitiveTokenToType(Token token)
    {
        return token switch
        {
            IntegerLiteral => Integer.TypeName,
            RealLiteral => Real.TypeName,
            BooleanLiteral => Boolean.TypeName,
            StringLiteral => String.TypeName,
            _ => null
        };
    }
    
    private static CodeExpression TokenToPrimitiveExpression(Token token)
    {
        return token switch
        {
            IntegerLiteral integer => new CodePrimitiveExpression(integer.Value),
            RealLiteral real => new CodePrimitiveExpression(real.Value),
            BooleanLiteral boolean => new CodePrimitiveExpression(bool.Parse(boolean.Literal)),
            StringLiteral str => new CodePrimitiveExpression(str.Literal),
            _ => new CodeTypeReferenceExpression(token.Literal)
        };
    }
}