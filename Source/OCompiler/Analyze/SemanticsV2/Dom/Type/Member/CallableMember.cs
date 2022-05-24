using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class CallableMember : TypeMember, ICanHaveParameters, ICanHaveStatements
{
    public List<ParameterDeclarationExpression> Parameters { get; } = new();

    public List<DomStatement> Statements { get; } = new();

    public Dictionary<string, TypeReference> Context { get; } = new();

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
    
    public bool SameSignatureAs(CallableMember other)
    {
        if (Name != other.Name)
        {
            return false;
        }
        
        if (Parameters.Count != other.Parameters.Count)
        {
            return false;
        }

        for (var i = 0; i < Parameters.Count; ++i)
        {
            var type = Parameters[i].Type;
            var otherType = other.Parameters[i].Type;
            if (type.DifferentFrom(otherType))
            {
                return false;
            }
        }

        return true;
    }
}