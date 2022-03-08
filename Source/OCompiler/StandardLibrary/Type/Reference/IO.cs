using System;

namespace OCompiler.StandardLibrary.Type.Reference;

public static class IO
{
    static void Write(String str)
    {
        Console.Write(str.Value);
    }
    
    static void WriteLine(String str)
    {
        Console.WriteLine(str.Value);
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