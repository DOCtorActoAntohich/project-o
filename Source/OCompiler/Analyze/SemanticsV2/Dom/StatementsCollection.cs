using System.Collections;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom;

internal class StatementsCollection : IEnumerable<DomStatement>
{
    private readonly List<DomStatement> _body = new();

    public int Count => _body.Count;

    public CodeObject Holder { get; set; }
    
    public VariableTable VariableTable { get; }

    public StatementsCollection(CodeObject holder)
    {
        Holder = holder;
        VariableTable = new VariableTable(this);
    }

    public StatementsCollection(CodeObject holder, IEnumerable<DomStatement> statements) : this(holder)
    {
        AddRange(statements);
    }

    public void Add(DomStatement statement)
    {
        _body.Add(statement);
        statement.ParentBody = this;
    }

    public void AddRange(IEnumerable<DomStatement> statements)
    {
        foreach (var statement in statements)
        {
            Add(statement);
        }
    }

    public IEnumerator<DomStatement> GetEnumerator()
    {
        return _body.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public string ToString(string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < Count; ++i)
        {
            var isLast = (i + 1 == Count);
            var firstLinePrefix = isLast ? nestedPrefix + "└── " : nestedPrefix + "├── ";
            stringBuilder.Append(
                _body[i].ToString(firstLinePrefix, nestedPrefix + "│   "));
            if (!isLast)
            {
                stringBuilder.Append('\n');
            }
        }
        
        return stringBuilder.ToString();
    }
}