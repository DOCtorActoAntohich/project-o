using OCompiler.Pipeline;

namespace OCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new Compiler(sourceFilePath: args[0]).Run();
        }
    }
}
