using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics.Callable;

internal class ParsedMethodInfo : CallableInfo
{
    public Method Method => (Method)Callable;
    public string Name { get; }
    public string ReturnType { get; }

    public ParsedMethodInfo(Method parsedMethod) : base(parsedMethod)
    {
        Name = parsedMethod.Name.Literal;
        ReturnType = parsedMethod.ReturnType == null ? "Void" : parsedMethod.ReturnType.Literal;
    }
}
