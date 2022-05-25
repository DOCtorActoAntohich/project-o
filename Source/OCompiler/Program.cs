using OCompiler.Pipeline;

using System;

namespace OCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
                var assembly = new Compiler(sourceFilePath: args[0]).Run();
            try
            {

                Console.WriteLine("Program output:");
                new Invoker(assembly, args[1], args[2..]).Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception.GetType().Name}(\"{exception.Message}\")");
                throw;
            }
        }
    }
}
