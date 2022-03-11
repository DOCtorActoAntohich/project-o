using System.Collections.Generic;
using System.Linq;

using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;

namespace OCompiler.Analyze.Semantics;

internal class ParsedConstructorInfo
{
    public Constructor Constructor { get; }
    public Body Body => Constructor.Body;
    public List<ParsedParameterInfo> Parameters { get; }
    public ParsedConstructorInfo(Constructor parsedConstructor)
    {
        Constructor = parsedConstructor;
        Parameters = parsedConstructor.Parameters.Select(parameter => new ParsedParameterInfo(parameter)).ToList();
    }

    public List<string> GetParameterTypes()
    {
        return Parameters.Select(p => p.Type).ToList();
    }
}
