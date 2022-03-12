using OCompiler.Analyze.Syntax.Declaration.Class.Member;

namespace OCompiler.Analyze.Semantics.Callable;

internal class ParsedConstructorInfo : CallableInfo
{
    public Constructor Constructor => (Constructor)Callable;

    public ParsedConstructorInfo(Constructor parsedConstructor) : base(parsedConstructor) { }
}
