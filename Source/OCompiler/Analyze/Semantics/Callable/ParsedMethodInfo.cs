using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics.Callable;

internal class ParsedMethodInfo : CallableInfo
{
    public Method Method => (Method)Callable;
    public string Name { get; }
    public string ReturnType { get; }

    public ParsedMethodInfo(
        Method parsedMethod,
        Context context
    ) : base(parsedMethod, context)
    {
        Name = parsedMethod.Name.Literal;
        ReturnType = parsedMethod.ReturnType is null ? "Void" : parsedMethod.ReturnType.Name.Literal;
    }
}
