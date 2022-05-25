namespace OCompiler.Builtins.Primitives;

public class Integer : Class
{
    // System.Int32 (hopefully).
    internal int Value { get; }

    public Integer(int p)
    {
        Value = p;
    }

    internal Integer(double p)
    {
        Value = (int) p;
    }

    
    public Integer()
    {
        Value = 0;
    }
    
    public Integer(Integer p)
    {
        Value = p.Value;
    }

    public Integer(Real p)
    {
        Value = (int) p.Value;
    }


    public Integer Min() => new Integer(int.MinValue);
    public Integer Max() => new Integer(int.MaxValue);

    
    public Real ToReal()
    {
        return new Real(Value);
    }

    public Boolean ToBoolean()
    {
        return Value != 0 ?
            new Boolean(true) :
            new Boolean(false);
    }

    public new String ToString()
    {
        return new String(Value.ToString());
    }


    public Integer UnaryMinus()
    {
        return new Integer(-Value);
    }


    public Integer Plus(Integer p)
    {
        return new Integer(Value + p.Value);
    }

    public Real Plus(Real p)
    {
        return new Real(Value + p.Value);
    }
    
    
    public Integer Minus(Integer p)
    {
        return new Integer(Value - p.Value);
    }

    public Real Minus(Real p)
    {
        return new Real(Value - p.Value);
    }
    
    
    public Integer Mult(Integer p)
    {
        return new Integer(Value * p.Value);
    }

    public Real Mult(Real p)
    {
        return new Real(Value * p.Value);
    }
    
    
    public Integer Div(Integer p)
    {
        return new Integer(Value / p.Value);
    }

    public Real Div(Real p)
    {
        return new Real(Value / p.Value);
    }
    
    
    public Integer Mod(Integer p)
    {
        return new Integer(Value % p.Value);
    }

    public Real Mod(Real p)
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