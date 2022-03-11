using System.Collections.Generic;
using System.Linq;

using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics;

internal class ParsedMethodInfo
{
    public Method Method { get; }
    public string Name { get; }
    public Body Body => Method.Body;
    public string ReturnType { get; }
    public List<ParsedParameterInfo> Parameters { get; }
    public ParsedMethodInfo(Method parsedMethod)
    {
        Name = parsedMethod.Name.Literal;
        Method = parsedMethod;
        ReturnType = parsedMethod.ReturnType == null ? "Void" : parsedMethod.ReturnType.Literal;
        Parameters = Method.Parameters.Select(parameter => new ParsedParameterInfo(parameter)).ToList();
    }

    public List<string> GetParameterTypes()
    {
        return Parameters.Select(p => p.Type).ToList();
    }
}
