using System.Globalization;

namespace OCompiler.Builtins.Primitives;

// Get real lol.
public class Real : Class
{
    internal double Value { get; }
    
    public Real(double p)
    {
        Value = p;
    }

    
    public Real()
    {
        Value = 0.0;
    }
    
    public Real(Real p)
    {
        Value = p.Value;
    }

    public Real(Integer p)
    {
        Value = p.Value;
    }
    
    
    public Real Min => new Real(double.MinValue);
    public Real Max => new Real(double.MaxValue);

    public Real Epsilon => new Real(double.Epsilon);


    public Integer ToInteger()
    {
        return new Integer((int) Value);
    }
    
    public Boolean ToBoolean()
    {
        return Value is < -double.Epsilon or > double.Epsilon ?
            new Boolean(true) :
            new Boolean(false);
    }

    public new String ToString()
    {
        return new String(Value.ToString(CultureInfo.CurrentCulture));
    }
    
    
    public Real Plus(Integer p)
    {
        return new Real(Value + p.Value);
    }

    public Real Plus(Real p)
    {
        return new Real(Value + p.Value);
    }
    
    
    public Real Minus(Integer p)
    {
        return new Real(Value - p.Value);
    }

    public Real Minus(Real p)
    {
        return new Real(Value - p.Value);
    }
    
    
    public Real Mult(Integer p)
    {
        return new Real(Value * p.Value);
    }

    public Real Mult(Real p)
    {
        return new Real(Value * p.Value);
    }
    
    
    public Real Div(Integer p)
    {
        return new Real(Value / p.Value);
    }

    public Real Div(Real p)
    {
        return new Real(Value / p.Value);
    }
    
    
    public Real Mod(Integer p)
    {
        return new Real(Value % p.Value);
    }
    
    
    public Boolean Less(Integer p)
    {
        return new Boolean(Value < p.Value);
    }
    
    public Boolean Less(Real p)
    {
        return new Boolean(Value < p.Value);
    }
    
    
    public Boolean LessEqual(Integer p)
    {
        return new Boolean(Value <= p.Value);
    }
    
    public Boolean LessEqual(Real p)
    {
        return new Boolean(Value <= p.Value);
    }
    
    
    public Boolean Greater(Integer p)
    {
        return new Boolean(Value > p.Value);
    }
    
    public Boolean Greater(Real p)
    {
        return new Boolean(Value > p.Value);
    }
    
    
    public Boolean GreaterEqual(Integer p)
    {
        return new Boolean(Value >= p.Value);
    }
    
    public Boolean GreaterEqual(Real p)
    {
        return new Boolean(Value >= p.Value);
    }
    
    
    public Boolean Equal(Integer p)
    {
        return new Boolean(Value.CompareTo(p.Value) == 0);
    }
    
    public Boolean Equal(Real p)
    {
        return new Boolean(Value.CompareTo(p.Value) == 0);
    }
}