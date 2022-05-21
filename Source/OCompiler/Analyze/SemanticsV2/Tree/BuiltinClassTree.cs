using System;
using System.Collections.Generic;
using System.Reflection;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Tree;

internal class BuiltinClassTree
{
    public Dictionary<string, ClassDeclaration> Classes { get; } = new();

    public BuiltinClassTree()
    {
        foreach (var @class in BuiltinClassesFromAssembly())
        {
            if (HasBuiltinClass(@class))
            {
                continue;
            }
            CreateBuiltinClass(@class);
        }
    }
    
    // Because C# goes like:
    // "Dict`2" idiot
    private static string TrimGrave(string typeName)
    {
        var graveIndex = typeName.IndexOf('`');
        return graveIndex >= 0 ? typeName[..graveIndex] : typeName;
    }

    private bool HasBuiltinClass(Type type)
    {
        return Classes.ContainsKey(TrimGrave(type.Name));
    }
    
    private void CreateBuiltinClass(Type type)
    {
        if (type.BaseType != null && type.BaseType != typeof(object) && !HasBuiltinClass(type.BaseType))
        {
            CreateBuiltinClass(type.BaseType);
        }

        var declaration = new ClassDeclaration(TrimGrave(type.Name))
        {
            DotnetType = type
        };
        AddClassDeclaration(declaration);

        if (type.BaseType != null)
        {
            declaration.BaseType = new TypeReference(TrimGrave(type.BaseType.Name))
            {
                DotnetType = type.BaseType
            };
        }
        
        AddFieldsForBuiltin(declaration, type);
        AddMethodsForBuiltin(declaration, type);
        AddConstructorsForBuiltin(declaration, type);
    }

    private void AddClassDeclaration(ClassDeclaration declaration)
    {
        Classes.Add(declaration.Name, declaration);
    }

    private void AddFieldsForBuiltin(ClassDeclaration declaration, Type builtinClass)
    {
        foreach (var field in builtinClass.GetRuntimeFields())
        {
            var type = new TypeReference(field.FieldType.Name)
            {
                DotnetType = field.FieldType
            };
            var memberField = new MemberField(field.Name, type)
            {
                DotnetType = field
            };
            declaration.AddField(memberField);
        }
    }

    private void AddMethodsForBuiltin(ClassDeclaration declaration, Type builtinClass)
    {
        foreach (var method in builtinClass.GetRuntimeMethods())
        {
            var parameters = ExtractParameters(method);
            var returnType = new TypeReference(method.ReturnType.Name)
            {
                DotnetType = method.ReturnType
            };
            var memberMethod = new MemberMethod(method.Name, parameters, returnType)
            {
                DotnetType = method
            };

            declaration.AddMethod(memberMethod);
        }
    }

    private void AddConstructorsForBuiltin(ClassDeclaration declaration, Type builtinClass)
    {
        foreach (var constructor in builtinClass.GetConstructors())
        {
            var parameters = ExtractParameters(constructor);
            var memberConstructor = new MemberConstructor(parameters)
            {
                DotnetType = constructor
            };

            declaration.AddConstructor(memberConstructor);
        }
    }

    private IEnumerable<ParameterDeclarationExpression> ExtractParameters(MethodBase callable)
    {
        var parameters = new List<ParameterDeclarationExpression>();
        foreach (var parameter in callable.GetParameters())
        {
            var type = new TypeReference(parameter.ParameterType.Name)
            {
                DotnetType = parameter.ParameterType
            };
            var name = parameter.Name ?? "";
            parameters.Add(new ParameterDeclarationExpression(name, type));
        }

        return parameters;
    }
    
    private static List<Type> BuiltinClassesFromAssembly(string @namespace = "OCompiler.Builtins")
    {
        var standardClasses = new List<Type>();
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var isClass = type.IsClass || type.IsValueType;
            var isInNamespace = type.Namespace != null && type.Namespace.StartsWith(@namespace);
            if (isClass && isInNamespace)
            {
                standardClasses.Add(type);
            }
        }

        return standardClasses;
    }
}