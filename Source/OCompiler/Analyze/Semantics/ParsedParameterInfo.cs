using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics;

internal class ParsedParameterInfo
{
    public Parameter Parameter { get; }
    public string Name => Parameter.Name.Literal;
    public string Type => Parameter.Type.Literal;

    public ParsedParameterInfo(Parameter parsedParameter)
    {
        Parameter = parsedParameter;
    }
}
