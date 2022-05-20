using System;
using OCompiler.StandardLibrary.Type.Value;
using Void = OCompiler.StandardLibrary.Type.Value.Void;

namespace OCompiler.StandardLibrary.Type;

public class Time : Class
{
    public Integer Current()
    {
        return new Integer(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    public Void Sleep(Integer seconds)
    {
        System.Threading.Thread.Sleep(seconds.Value * 1000);
        return new Void();
    }
}
