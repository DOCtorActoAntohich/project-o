using System.CodeDom;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomInt    = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal   = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomVoid   = OCompiler.StandardLibrary.CodeDom.Value.Void;

namespace OCompiler.StandardLibrary.CodeDom.Reference;

internal static class Time
{
    public const string TypeName = "Time";
    public const string FullTypeName = $"{Base.Namespace}.{TypeName}";
    
    public static CodeTypeDeclaration Generate()
    {
        var timeType = Base.GenerateWithDefaultToString(TypeName);
        timeType.BaseTypes.Add(new CodeTypeReference(DomAnyRef.FullTypeName));
        
        timeType.AddCurrentTimeMethod();
        timeType.AddSleepMethod();

        return timeType;
    }

    private static void AddCurrentTimeMethod(this CodeTypeDeclaration timeType)
    {
        const string currentTimeMethodName = "Current";

        var time = new CodeTypeReference(typeof(System.DateTime));
        var timeExpression = new CodeTypeReferenceExpression(time);

        var theParametersForTheBeginning = new CodeExpression[]
        {
            new CodePrimitiveExpression(1970),
            new CodePrimitiveExpression(1),
            new CodePrimitiveExpression(1)
        };
        var whenTheUniverseStartedExisting =
            new CodeObjectCreateExpression(time, theParametersForTheBeginning);

        var now = new CodePropertyReferenceExpression(timeExpression, "Now");
        var timeDifference = new CodeMethodInvokeExpression(
            now, "Subtract", whenTheUniverseStartedExisting);

        var @double = new CodePropertyReferenceExpression(timeDifference, "TotalSeconds");
        var convert = new CodeTypeReferenceExpression(typeof(System.Convert));
        var @int = new CodeMethodInvokeExpression(@convert, "ToInt32", @double);
            
        var returnValue = new CodeObjectCreateExpression(DomInt.FullTypeName, @int);
        var returnStatement = new CodeMethodReturnStatement(returnValue);

        var currentTimeMethod = Base.EmptyPublicMethod(DomInt.FullTypeName, currentTimeMethodName);
        currentTimeMethod.Statements.Add(returnStatement);

        timeType.Members.Add(currentTimeMethod);
    }
    
    private static void AddSleepMethod(this CodeTypeDeclaration timeType)
    {
        const string sleepMethodName = "Sleep";
        const string paramName = "ms";

        var ms = new CodeArgumentReferenceExpression(paramName);
        var @double = new CodeFieldReferenceExpression(ms, Base.InternalValueVariableName);
        
        var thread = new CodeTypeReferenceExpression(typeof(System.Threading.Thread));
        var sleepStatement = new CodeMethodInvokeExpression(thread, sleepMethodName, @double);

        var sleepMethod = Base.EmptyPublicMethod(DomVoid.FullTypeName, sleepMethodName);
        sleepMethod.Parameters.Add(new CodeParameterDeclarationExpression(DomInt.FullTypeName, paramName));
        
        sleepMethod.Statements.Add(sleepStatement);
        sleepMethod.Statements.Add(Base.ReturnVoid());

        timeType.Members.Add(sleepMethod);
    }
}