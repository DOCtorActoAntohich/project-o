using OCompiler.Pipeline;

namespace OCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var assembly = new Compiler(sourceFilePath: args[0]).Run();
            new Invoker(assembly, args[1], args[2..]).Run();
        }
    }
}
