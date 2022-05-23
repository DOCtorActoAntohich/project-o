using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class CallableMember : TypeMember, ICanHaveParameters, ICanHaveStatements
{
    public List<ParameterDeclarationExpression> Parameters { get; } = new();

    public List<DomStatement> Statements { get; } = new();
    
    public CallableMember(string name) : base(name)
    {
    }

    public void AddParameter(ParameterDeclarationExpression parameter)
    {
        Parameters.Add(parameter);
        parameter.Holder = this;
    }

    public void AddParameters(IEnumerable<ParameterDeclarationExpression> parameters)
    {
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }
}