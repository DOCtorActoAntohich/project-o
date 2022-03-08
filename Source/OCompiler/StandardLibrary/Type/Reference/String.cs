using System.Globalization;
using OCompiler.StandardLibrary.Type.Value;

namespace OCompiler.StandardLibrary.Type.Reference;

public class String : AnyRef
{
    internal string Value { get; }

    internal String(string p)
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
        Value = p.Value.ToString();
    }


    public Integer ToInt()
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


    public Integer At(Integer index)
    {
        return new Integer(Value[index.Value]);
    }

    public String Concatenate(String other)
    {
        return new String(Value + other.Value);
    }
}