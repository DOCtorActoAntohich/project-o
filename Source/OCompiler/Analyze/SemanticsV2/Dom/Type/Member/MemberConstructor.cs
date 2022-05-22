using System.Collections.Generic;
using System.Linq;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberConstructor : TypeMember, ICanHaveParameters, ICanHaveStatements
{
    public List<ParameterDeclarationExpression> Parameters { get; } = new();
    
    public List<DomStatement> Statements { get; } = new();
    
    
    public MemberConstructor(string name = "") : base(name)
    {
    }

    public MemberConstructor(IEnumerable<ParameterDeclarationExpression> parameters, string name = "") : this(name)
    {
        Parameters.AddRange(parameters);
    }

    public void AddParameter(ParameterDeclarationExpression parameter)
    {
        Parameters.Add(parameter);
        parameter.Holder = this;
    }

    public override string ToString()
    {
        return new StringBuilder(Owner?.Name ?? Name)
            .Append($"::{Name}")
            .Append('(')
            .Append(string.Join(", ", Parameters))
            .Append(')')
            .ToString();
    }
}