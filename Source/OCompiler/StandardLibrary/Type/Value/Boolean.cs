using OCompiler.StandardLibrary.Type.Reference;

namespace OCompiler.StandardLibrary.Type.Value;

public readonly struct Boolean : AnyValue
{
    internal bool Value { get; }

    internal Boolean(bool p)
    {
        Value = p;
    }


    public Boolean(Boolean p)
    {
        Value = p.Value;
    }


    public Integer ToInteger()
    {
        return Value ?
            new Integer(1) :
            new Integer(0);
    }

    public new String ToString()
    {
        return new String(Value.ToString());
    }


    public Boolean Or(Boolean p)
    {
        return new Boolean(Value | p.Value);
    }

    public Boolean And(Boolean p)
    {
        return new Boolean(Value & p.Value);
    }

    public Boolean Xor(Boolean p)
    {
        return new Boolean(Value ^ p.Value);
    }

    public Boolean Not()
    {
        return new Boolean(!Value);
    }
}