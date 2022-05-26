using OCompiler.Exceptions;

using System.Collections.Generic;
using System.Reflection.Emit;

namespace OCompiler.Generate;

internal class ScopeV2
{
    public int StackSize { get; private set; }

    private ScopeV2? _parent;
    private Dictionary<string, LocalBuilder> _variables;

    public ScopeV2(): this(null) { }
    public ScopeV2(ScopeV2? parent)
    {
        _parent = parent;
        _variables = new Dictionary<string, LocalBuilder>();
    }

    public ScopeV2 GetChild()
    {
        return new ScopeV2(this);
    }

    public LocalBuilder GetVariable(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return _variables[name];
        }

        if (_parent is not null)
        {
            return _parent.GetVariable(name);
        }

        throw new CompilerInternalError($"Variable {name} is not defined.");
    }

    public void SetVariable(string name, LocalBuilder variable)
    {
        if (_variables.ContainsKey(name))
        {
            throw new CompilerInternalError($"Variable {name} already defined.");
        }
        
        _variables.Add(name, variable);
    }
    
    public void IncreaseStackSize()
    {
        StackSize++;
    }

    public void DecreaseStackSize()
    {
        if (StackSize == 0)
        {
            throw new CompilerInternalError("Stack size is zero.");
        }

        StackSize--;
    }
}