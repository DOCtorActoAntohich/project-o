using OCompiler.Pipeline;

using System;

namespace OCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var olangFile = args[0];
            var exePath = args[1];
            var mainClass = args[2];
            new Compiler(olangFile, exePath, mainClass).Run();
        }
        
        /*private static void Main_bad(string[] args)
        {
            var assembly = new Compiler(sourceFilePath: args[0]).Run();

            Console.WriteLine("Program output:");
            new Invoker(assembly, args[1], args[2..]).Run();
        }*/
    }
}
