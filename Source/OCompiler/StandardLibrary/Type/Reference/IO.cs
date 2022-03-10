using System;
using Void = OCompiler.StandardLibrary.Type.Value.Void;

namespace OCompiler.StandardLibrary.Type.Reference;

public static class IO
{
    static Void Write(String str)
    {
        Console.Write(str.Value);
        return new Void();
    }
    
    static Void WriteLine(String str)
    {
        Console.WriteLine(str.Value);
        return new Void();
    }

    static String ReadLine()
    {
        var result = Console.ReadLine();
        if (result == null)
        {
            return new String();
        }

        return new String(result);
    }
}