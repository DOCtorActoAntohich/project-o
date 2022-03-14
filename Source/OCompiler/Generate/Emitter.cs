using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.Generate;

internal class Emitter
{
    private AssemblyName _assemblyName;
    private AssemblyBuilder _assemblyBuilder;
    private ModuleBuilder _moduleBuilder;
    
    // Standard library.
    private Dictionary<string, Type> _standardTypes;

    // To compile.
    private Dictionary<string, TypeBuilder> _typeBuilders;
    private Dictionary<string, Dictionary<Type[], ConstructorBuilder>> _constructorBuilders;
    private Dictionary<string, Dictionary<string, Dictionary<Type[], MethodBuilder>>> _methodBuilders;

    public Emitter(List<ClassInfo> classes)
    {
        // Define assembly.
        _assemblyName = new AssemblyName("O");
        _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Runtime");
    
        // Standard library.
        _standardTypes = new Dictionary<string, Type>();

        // Initialize standard library.
        foreach (var (name, @class) in BuiltClassInfo.StandardClasses)
        {
            _standardTypes.Add(name, (Type)@class.Class!);
        }
        
        // To compile.
        _typeBuilders = new Dictionary<string, TypeBuilder>();
        _constructorBuilders = new Dictionary<string, Dictionary<Type[], ConstructorBuilder>>();
        _methodBuilders = new Dictionary<string, Dictionary<string, Dictionary<Type[], MethodBuilder>>>();
        
        // Define classes.
        foreach (var @class in classes)
        {
            if (@class is ParsedClassInfo parsedClass)
            {
                DefineType(parsedClass);
            }
        }
        
        // Define constructors.
        foreach (var @class in classes)
        {
            if (@class is not ParsedClassInfo)
            {
                continue;
            }
        
            var parsedClass = (ParsedClassInfo) @class;
            foreach (var constructor in parsedClass.Constructors)
            {
                DefineConstructor(constructor);
            }
        }
        
        // Define methods and fields.
        foreach (var @class in classes)
        {
            if (@class is not ParsedClassInfo)
            {
                continue;
            }
        
            var parsedClass = (ParsedClassInfo) @class;
            
            foreach (var method in parsedClass.Methods)
            {
                DefineMethod(method);
            }
            
            foreach (var field in parsedClass.Fields)
            {
                DefineField(field);
            }
        }
        
        // Define methods and constructors bodies.
        // Define methods and fields.
        foreach (var @class in classes)
        {
            if (@class is not ParsedClassInfo)
            {
                continue;
            }
        
            var parsedClass = (ParsedClassInfo) @class;
            
            foreach (var constructor in parsedClass.Constructors)
            {
                EmitConstructor(constructor);
            }
            
            foreach (var method in parsedClass.Methods)
            {
                EmitMethod(method);
            }
        }
    }

    public void Run(string entryPointType = "Main", object[]? args = null)
    {
        Activator.CreateInstance(_typeBuilders["Main"].CreateType()!, args);
    }
    
    private Type? GetType(string name)
    {
        if (_standardTypes.ContainsKey(name))
        {
            return _standardTypes[name];
        }
        
        if (_typeBuilders.ContainsKey(name))
        {
            return _typeBuilders[name];
        }

        return null;
    }

    private MethodInfo? GetMethod(string @class, string name, Type[] parameters)
    {
        return GetType(@class)?.GetMethod(name, BindingFlags.Public, parameters);
    }

    private ConstructorInfo? GetConstructor(string @class, Type[] parameters)
    {
        return GetType(@class)?.GetConstructor(parameters);
    }

    private Type[] GetParameters(List<ParsedParameterInfo> parameters)
    {
        // Get types.
        var parameterTypes = new Type[parameters.Count];
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            parameterTypes[i] = GetType(parameters[i].Type) ?? throw new Exception(
                $"Type not found: {parameters[i].Type}"
            );
        }

        return parameterTypes;
    }
    
    private void DefineType(ParsedClassInfo @class)
    {
        // Already defined.
        if (GetType(@class.Name) is not null)
        {
            return;
        }
        
        // If class not from standard library, make sure that it is defined.
        if (@class.BaseClass is ParsedClassInfo parentClass)
        {
            DefineType(parentClass);
        }

        // Add to builders.
        _typeBuilders.Add(
            @class.Name,
            _moduleBuilder.DefineType(
                @class.Name,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                @class.BaseClass is not null ? GetType(@class.BaseClass.Name) : null
            )
        );
        
        // Initialize methods storage.
        if (!_methodBuilders.ContainsKey(@class.Name))
        {
            _methodBuilders.Add(@class.Name, new Dictionary<string, Dictionary<Type[], MethodBuilder>>());
        }
        
        // Initialize constructors storage.
        if (!_constructorBuilders.ContainsKey(@class.Name))
        {
            _constructorBuilders.Add(@class.Name, new Dictionary<Type[], ConstructorBuilder>(
                new Comparator())
            );
        }
    }
    
    private void DefineConstructor(ParsedConstructorInfo constructor)
    {
        // Get types.
        var parameterTypes = GetParameters(constructor.Parameters);
        
        // Check if type exists and has a builder.
        Type? type = GetType(constructor.Context.CurrentClass.Name);
        if (type is not TypeBuilder)
        {
            throw new Exception($"Defining constructor on invalid builder: {type}.");
        }

        // Define.
        _constructorBuilders[type.Name].Add(
            parameterTypes,
            ((TypeBuilder) type).DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                parameterTypes
            )
        );
    }

    
    private void DefineMethod(ParsedMethodInfo method)
    {
        // Get types.
        var parameterTypes = GetParameters(method.Parameters);
        
        // Check if type exists and has a builder.
        Type? type = GetType(method.Context.CurrentClass.Name);
        if (type is not TypeBuilder)
        {
            throw new Exception($"Defining method on invalid builder: {type}.");
        }
        
        // Initialize methods storage.
        if (!_methodBuilders[type.Name].ContainsKey(method.Name))
        {
            _methodBuilders[type.Name].Add(method.Name, new Dictionary<Type[], MethodBuilder>(
                new Comparator())
            );
        }
        
        // Define.
        _methodBuilders[type.Name][method.Name].Add(
            parameterTypes,
            ((TypeBuilder) type).DefineMethod(
                method.Name,
                MethodAttributes.Public,
                CallingConventions.HasThis,
                GetType(method.ReturnType) ?? throw new Exception($"Type not found: {method.ReturnType}"),
                parameterTypes
            )
        );
    }

    private void DefineField(ParsedFieldInfo field)
    {
        if (field.Type == null)
        {
            throw new Exception(
                $"Field {field.Name} in class {field.Context.CurrentClass.Name} has no Type."
            );
        }
        
        // Check if type exists and has a builder.
        Type? type = GetType(field.Context.CurrentClass.Name);
        if (type is not TypeBuilder)
        {
            throw new Exception($"Defining method on invalid builder: {type}.");
        }
        
        ((TypeBuilder)type).DefineField(
            field.Name, 
            GetType(field.Type) ?? throw new Exception($"Type not found: {field.Type}"),
            FieldAttributes.Public
        );
    }

    private void EmitConstructor(ParsedConstructorInfo constructor)
    {
        var parameterTypes = GetParameters(constructor.Parameters);
        var builder = _constructorBuilders[constructor.Context.CurrentClass.Name][parameterTypes];
        builder.GetILGenerator().Emit(OpCodes.Ret);
    }
    
    private void EmitMethod(ParsedMethodInfo method)
    {
        
    }

    private void EmitBody(Body methodBody, ILGenerator generator)
    {
        foreach (var statement in methodBody)
        {
            Emit(statement, generator);
        }
    }

    private void Emit(IBodyStatement statement, ILGenerator generator)
    {
        switch (statement)
        {
            case Variable variable:
                // TODO
                break;
            case Assignment assignment:
                // TODO
                break;
            case If conditional:
                // TODO
                break;
            case Return @return:
                // TODO
                break;
            case While loop:
                // TODO
                break;
            case Call call:
                // TODO
                break;;
            case Expression expression:
                // TODO
                break;
            default:
                throw new Exception($"Unknown IBodyStatement: {statement}");
        }
    }
}
