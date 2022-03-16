using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace OCompiler.CodeGeneration.Translation.CSharp;

internal class CodeGenerator
{
    private readonly MemoryStream _compiledAssembly;
    private EmitResult _result = null!;
    public bool Success => _result.Success;

    public CodeGenerator(Code code)
    {
        _compiledAssembly = new MemoryStream();
        CompileCode(code);
    }

    ~CodeGenerator()
    {
        _compiledAssembly.Close();
    }

    public void Build(string exePath)
    {
        var fullPath = Path.GetFullPath(exePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null)
        {
            Directory.CreateDirectory(directory);
        }

        SaveExeFile(fullPath);
        SaveRuntimeConfig(fullPath);
        CopyReferencedAssemblies(fullPath);
    }

    private void SaveExeFile(string exePath)
    {
        using var file = new FileStream(exePath, FileMode.Create, FileAccess.Write);
        
        _compiledAssembly.Seek(0, SeekOrigin.Begin);
        _compiledAssembly.CopyTo(file);
    }

    private static void SaveRuntimeConfig(string exePath)
    {
        var rootDirectory = Path.GetDirectoryName(exePath) ?? "";
        var fileName = Path.GetFileNameWithoutExtension(exePath);
        var jsonPath = Path.Combine(rootDirectory, fileName);
        jsonPath = $"{jsonPath}.runtimeconfig.json";
        
        using var jsonFile = File.Create(jsonPath);
        var options = new JsonWriterOptions
        {
            Indented = true
        };
        using var writer = new Utf8JsonWriter(jsonFile, options);
        
        writer.WriteStartObject();
        writer.WriteStartObject("runtimeOptions");
        writer.WriteString("tfm", "net6.0"); // TODO: make versions more flexible.
        writer.WriteStartObject("framework");
        writer.WriteString("name", "Microsoft.NETCore.App");
        writer.WriteString(
            "version",
            RuntimeInformation.FrameworkDescription.Replace(".NET ", "")
        );
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private static void CopyReferencedAssemblies(string exePath)
    {
        var exeDirectory = Path.GetDirectoryName(exePath) ?? "";
        
        var referencedPaths = GetPathsToReferencedAssemblies();
        foreach (var sourcePath in referencedPaths)
        {
            var name = Path.GetFileName(sourcePath);
            var destination = Path.Combine(exeDirectory, name);
            File.Copy(sourcePath, destination, overwrite: true);
        }
    }

    
    private void CompileCode(Code code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code.Text);

        var references = GetReferencedAssemblies();
        var assemblyName = Path.GetRandomFileName();
        var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
        
        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            options
        );
        
        _result = compilation.Emit(_compiledAssembly);
        if (Success)
        {
            return;
        }
        
        ReportErrorsOnFailure();
    }

    private void ReportErrorsOnFailure()
    {
        Console.Error.Write("\nCOMPILATION FAILED\n");
        Console.Error.Write("If you see this text, compiler developers messed up\n");
        Console.Error.Write("Here's a list of C# compilation errors.\n");
        
        var failures = _result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError ||
                                                               diagnostic.Severity == DiagnosticSeverity.Error);

        foreach (var diagnostic in failures)
        {
            Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
        }
    }

    
    private static IEnumerable<string> GetPathsToReferencedAssemblies()
    {
        var systemRuntimePath = Path.GetDirectoryName(
            typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location
        ) ?? "";
        
        var referencedPaths = new List<string> {
            Path.Combine(systemRuntimePath, "System.Runtime.dll"),
            typeof(object).GetTypeInfo().Assembly.Location,
            typeof(Console).GetTypeInfo().Assembly.Location,
        };
        
        // Load assemblies used in the current one (in this project).
        // Might not be needed cos OLang is too lightweight.
        
        // foreach (var assembly in Assembly.GetEntryAssembly()?.GetReferencedAssemblies()!)
        // {
        //     var loadedAssembly = Assembly.Load(assembly);
        //     var path = loadedAssembly.Location;
        //     if (!referencedPaths.Contains(path))
        //     {
        //         referencedPaths.Add(path);
        //     }
        // }

        return referencedPaths.Distinct().ToList();
    }

    private static IEnumerable<PortableExecutableReference> GetReferencedAssemblies()
    {
        return GetPathsToReferencedAssemblies().Select(
            path => MetadataReference.CreateFromFile(path)
        ).ToList();
    }
}