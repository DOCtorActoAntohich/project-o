namespace OCompiler.StandardLibrary.Type.Value;

public readonly struct Boolean
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