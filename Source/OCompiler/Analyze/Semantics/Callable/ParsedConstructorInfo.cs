using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;

using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.Callable;

internal class ParsedConstructorInfo : CallableInfo
{
    public Constructor Constructor => (Constructor)Callable;

    public ParsedConstructorInfo(Constructor parsedConstructor, Context context) : base(parsedConstructor, context) { }
}
