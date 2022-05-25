using System;
using System.Collections;
using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.SemanticsV2.Dom;

internal class VariableTable : IEnumerable<KeyValuePair<string, TypeReference>>
{
    private readonly Dictionary<string, TypeReference> _variables = new();

    private readonly VariableTable? _parent;
    
    public StatementsCollection Body { get; set; }

    public VariableTable(StatementsCollection body)
    {
        Body = body;
        
        _parent = Body.Holder switch
        {
            ConditionStatement @if when @if.Statements == Body => @if.Statements.VariableTable,
            ConditionStatement @if => @if.ElseStatements.VariableTable,
            LoopStatement @while => @while.Statements.VariableTable,
            CallableMember callable => callable.Statements.VariableTable,
            _ => null
        };
    }

    public bool Has(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return true;
        }

        return _parent?.Has(name) ?? false;
    }

    public TypeReference GetType(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return _variables[name];
        }

        return _parent?.GetType(name) ?? throw new CompilerInternalError($"No such field: {name}");
    }

    public void Add(string name, TypeReference type)
    {
        _variables.Add(name, type);
    }
    
    public IEnumerator<KeyValuePair<string, TypeReference>> GetEnumerator()
    {
        return _variables.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}