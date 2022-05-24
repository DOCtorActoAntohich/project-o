using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal interface ICanHaveParameters
{
    public List<ParameterDeclarationExpression> Parameters { get; }
}