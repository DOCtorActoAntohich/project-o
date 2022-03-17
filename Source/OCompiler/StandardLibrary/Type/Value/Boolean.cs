using OCompiler.StandardLibrary.Type.Reference;

namespace OCompiler.StandardLibrary.Type.Value;

public class Boolean : AnyValue
{
    public bool _value;
    
    public Boolean()
    {
        _value = false;
    }
    
    public Boolean(bool p)
    {
        _value = p;
    }


    public Boolean(Boolean p)
    {
        _value = p._value;
    }


    public Integer ToInteger()
    {
        return _value ?
            new Integer(1) :
            new Integer(0);
    }

    public new String ToString()
    {
        return new String(_value.ToString());
    }


    public Boolean Or(Boolean p)
    {
        return new Boolean(_value | p._value);
    }

    public Boolean And(Boolean p)
    {
        return new Boolean(_value & p._value);
    }

    public Boolean Xor(Boolean p)
    {
        return new Boolean(_value ^ p._value);
    }

    public Boolean Not()
    {
        return new Boolean(!_value);
    }
}