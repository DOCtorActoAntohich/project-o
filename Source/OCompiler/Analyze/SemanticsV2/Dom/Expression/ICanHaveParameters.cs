using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal interface ICanHaveParameters
{
    public List<ParameterDeclarationExpression> Parameters { get; }
}