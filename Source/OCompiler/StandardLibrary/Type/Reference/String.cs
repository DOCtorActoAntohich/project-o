using System.Globalization;
using OCompiler.StandardLibrary.Type.Value;

namespace OCompiler.StandardLibrary.Type.Reference;

public class String : AnyRef
{
    internal string Value { get; }

    public String(string p)
    {
        Value = p;
    }


    public String()
    {
        Value = "";
    }
    
    public String(String p)
    {
        Value = p.Value;
    }

    public String(Integer p)
    {
        Value = p.Value.ToString();
    }

    public String(Real p)
    {
        Value = p.Value.ToString(CultureInfo.CurrentCulture);
    }

    public String(Boolean p)
    {
        Value = p._value.ToString();
    }

    
    public Boolean Equal(String other)
    {
        return new Boolean(Value == other.Value);
    }
    
    public Integer ToInteger()
    {
        return new Integer(int.Parse(Value));
    }

    public Real ToReal()
    {
        return new Real(double.Parse(Value));
    }

    public Boolean ToBoolean()
    {
        return new Boolean(bool.Parse(Value));
    }


    public String At(Integer index)
    {
        return new String(Value[index.Value..(index.Value + 1)]);
    }

    public String Concatenate(String other)
    {
        return new String(Value + other.Value);
    }

    public new string ToString()
    {
        return Value;
    }
}