using System;

using Void = OCompiler.Builtins.Primitives.Void;
using String = OCompiler.Builtins.Primitives.String;
using OCompiler.Builtins.Primitives;

namespace OCompiler.Builtins.Utility;

public class IO : Class
{
    public Void Write(String str)
    {
        Console.Write(str.Value);
        return new Void();
    }
    
    public Void WriteLine(String str)
    {
        Console.WriteLine(str.Value);
        return new Void();
    }

    public String ReadLine()
    {
        var result = Console.ReadLine();
        if (result == null)
        {
            return new String();
        }

        return new String(result);
    }
}