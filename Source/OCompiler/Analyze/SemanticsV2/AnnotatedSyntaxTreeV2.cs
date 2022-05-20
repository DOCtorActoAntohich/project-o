using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace OCompiler.Analyze.SemanticsV2;

internal class AnnotatedSyntaxTreeV2
{
    public static Dictionary<string, ClassInfo> BuiltinClasses { get; }
    public Dictionary<string, ClassInfo> ParsedClasses { get; } = new();
    
    
    public AnnotatedSyntaxTreeV2(Syntax.Tree syntaxTree)
    {
        
    }

    static AnnotatedSyntaxTreeV2()
    {
        BuiltinClasses = new Dictionary<string, ClassInfo>();
        var standardClasses = LoadStandardClasses();

        foreach (var @class in standardClasses.Values)
        {
            CreateBuiltinClass(@class);
        }
    }

    private static void CreateBuiltinClass(Type type)
    {
        if (type.BaseType != null && BuiltinClasses.ContainsKey(type.BaseType.Name))
        {
            CreateBuiltinClass(type.BaseType);
        }

        var classInfo = new ClassInfo
        {
            Name = type.Name,
            Parent = type.BaseType == null ? null : BuiltinClasses[type.BaseType.Name]
        };
        
        
        
        BuiltinClasses.Add(classInfo.Name, classInfo);
    }
    
    private static Dictionary<string, Type> LoadStandardClasses(string @namespace = "OCompiler.StandardLibrary")
    {
        var standardClasses = new Dictionary<string, Type>();
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var isClass = type.IsClass || type.IsValueType;
            var isInNamespace = type.Namespace != null && type.Namespace.StartsWith(@namespace);
            if (isClass && isInNamespace)
            {
                standardClasses.Add(type.Name, type);
            }
        }

        return standardClasses;
    }
}