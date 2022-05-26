using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal interface ICanHaveArguments
{
    public List<Expression> Arguments { get; } 
}