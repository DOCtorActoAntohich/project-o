using System.Collections;
using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type;

namespace OCompiler.Analyze.SemanticsV2.Dom;

internal class VariableTable : IEnumerable<KeyValuePair<string, TypeReference>>
{
    private readonly Dictionary<string, TypeReference> _variables = new();

    public StatementsCollection Body { get; set; }

    public VariableTable(StatementsCollection body)
    {
        Body = body;
    }

    public bool Has(string name)
    {
        return _variables.ContainsKey(name);
    }

    public TypeReference GetType(string name)
    {
        return _variables[name];
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