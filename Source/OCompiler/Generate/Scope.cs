using OCompiler.Exceptions;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OCompiler.Generate;

internal class Scope
{
    public int StackSize { get; private set; }
    public Scope? Parent { get; }
    public Dictionary<string, LocalBuilder> Locals { get; }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
        Locals = new Dictionary<string, LocalBuilder>();
    }

    public Scope Child => new Scope(this);

    public LocalBuilder? Get(string name)
    {
        if (Locals.ContainsKey(name))
        {
            return Locals[name];
        }

        if (Parent is not null)
        {
            return Parent.Get(name);
        }

        return null;
    }

    public void Push()
    {
        StackSize++;
    }

    public void Pop()
    {
        if (StackSize == 0)
        {
            throw new CompilationError("Stack size is zero.");
        }

        StackSize--;
    }
}
