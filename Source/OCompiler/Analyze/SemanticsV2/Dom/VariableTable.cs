using System;
using System.Collections;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom;

internal class VariableTable : IEnumerable<KeyValuePair<string, TypeReference>>
{
    private readonly Dictionary<string, TypeReference> _variables = new();
    
    public StatementsCollection Body { get; set; }

    public VariableTable(StatementsCollection body)
    {
        Body = body;
    }

    private VariableTable? ParentTable()
    {
        var parentHolder = Body.Holder switch
        {
            ConditionStatement @if => @if.Holder,
            LoopStatement @while => @while.Holder,
            _ => null
        };
        if (parentHolder == null)
        {
            return null;
        }

        var parentStatement = parentHolder as DomStatement;

        return parentHolder switch
        {
            LoopStatement @while => @while.Statements.VariableTable,
            ConditionStatement @if when @if.Statements.Contains(parentStatement!) => @if.Statements.VariableTable,
            ConditionStatement @if => @if.ElseStatements.VariableTable,
            _ => null
        };
    }
    
    public bool Has(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return true;
        }

        return ParentTable()?.Has(name) ?? false;
    }

    public TypeReference GetType(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return _variables[name];
        }

        return ParentTable()?.GetType(name) ?? throw new CompilerInternalError($"No such field: {name}");
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