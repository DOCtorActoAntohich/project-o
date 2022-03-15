using OCompiler.Pipeline;

namespace OCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new Compiler(sourceFilePath: args[0], args[1], args[2..]).Run();
        }
    }
}
