using System;
using System.Collections;
using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom;

internal class ParametersCollection : IEnumerable<ParameterDeclarationExpression>
{
    private readonly List<ParameterDeclarationExpression> _parameters;
    
    public int Count => _parameters.Count;
    
    public CallableMember Holder { get; set; }
    

    public ParametersCollection(CallableMember holder)
    {
        _parameters = new List<ParameterDeclarationExpression>();
        Holder = holder;
    }

    public ParametersCollection(CallableMember holder, IEnumerable<ParameterDeclarationExpression> parameters)
        : this(holder)
    {
        AddRange(parameters);
    }

    public void Add(ParameterDeclarationExpression parameter)
    {
        _parameters.Add(parameter);
        parameter.ParentCollection = this;
    }

    public void AddRange(IEnumerable<ParameterDeclarationExpression> parameters)
    {
        foreach (var parameter in parameters)
        {
            Add(parameter);
        }
    }

    public bool SameAs(ParametersCollection other)
    {
        if (Count != other.Count)
        {
            return false;
        }

        for (var i = 0; i < Count; ++i)
        {
            var myParameter = _parameters[i];
            var otherParameter = other._parameters[i];
            if (myParameter.Type.DifferentFrom(otherParameter.Type))
            {
                return false;
            }
        }

        return true;
    }

    public bool DifferentFrom(ParametersCollection other)
    {
        return !SameAs(other);
    }
    
    
    public IEnumerator<ParameterDeclarationExpression> GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join(", ", _parameters);
    }
}