using OCompiler.Pipeline;

namespace OCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            new Compiler(sourceFilePath: args[0]).Run();
        }
    }
}
