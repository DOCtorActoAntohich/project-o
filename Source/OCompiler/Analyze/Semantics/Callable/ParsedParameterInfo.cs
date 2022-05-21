using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics.Callable;

internal class ParsedParameterInfo
{
    public Parameter Parameter { get; }
    public string Name => Parameter.Name.Literal;

    // Won't work for generics
    public string Type => Parameter.Type.Name.Literal;

    public ParsedParameterInfo(Parameter parsedParameter)
    {
        Parameter = parsedParameter;
    }
}
