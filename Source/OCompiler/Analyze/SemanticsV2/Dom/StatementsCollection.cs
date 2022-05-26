using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public void InsertFieldInitialization(DomStatement statement)
    {
        _body.Insert(1, statement);
        statement.ParentBody = this;
    }

    public void InsertBaseCall(DomStatement baseCall)
    {
        _body.Insert(0, baseCall);
        baseCall.ParentBody = this;
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

    public bool Contains(DomStatement targetStatement)
    {
        return _body.Contains(targetStatement);
    }

    public DomStatement LastStatement()
    {
        return _body.Last();
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