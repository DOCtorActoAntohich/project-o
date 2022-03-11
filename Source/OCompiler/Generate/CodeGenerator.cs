using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Class;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace OCompiler.Generate
{
    internal class CodeGenerator
    {
        private readonly Dictionary<string, Type> allClasses = new();
        private readonly Dictionary<ParsedClassInfo, TypeBuilder> classBuilders = new();
        private readonly string _namespacePrefix;

        public CodeGenerator(List<ClassInfo> classes, string @namespace = "OCompiler.Result")
        {
            _namespacePrefix = @namespace;
            if (_namespacePrefix.Length > 0 && !_namespacePrefix.EndsWith("."))
            {
                _namespacePrefix += ".";
            }

            while (classes.Count > 0) {
                var classInfo = classes[0];
                switch (classInfo)
                {
                    case ParsedClassInfo info:
                        GetTypeBuilder(info);
                        break;
                    case BuiltClassInfo info:
                        allClasses.Add(info.Name, info.Class);
                        break;
                    default:
                        throw new Exception($"Unexpected classInfo type: {classInfo}");
                }
                classes.RemoveAt(0);
            }
            foreach (var (classInfo, builder) in classBuilders)
            {
                BuildType(classInfo, builder);
            }
        }

        private Type BuildType(ParsedClassInfo classInfo, TypeBuilder typeBuilder)
        {
            foreach (var field in classInfo.Fields)
            {
                CreateField(typeBuilder, field);
            }
            foreach (var method in classInfo.Methods)
            {
                CreateMethod(typeBuilder, method);
            }
            foreach (var constructor in classInfo.Constructors)
            {
                CreateConstructor(typeBuilder, constructor);
            }
            return typeBuilder.CreateType()!;
        }

        private void CreateField(TypeBuilder typeBuilder, ParsedFieldInfo field)
        {
            if (field.Type == null)
            {
                throw new Exception($"Field {field.Name} in class {typeBuilder.Name} has no Type");
            }
            var fieldBuilder = typeBuilder.DefineField(field.Name, GetTypeByName(field.Type), FieldAttributes.Public);
        }

        private void CreateMethod(TypeBuilder typeBuilder, ParsedMethodInfo method)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public,
                CallingConventions.HasThis,
                GetTypeByName(method.ReturnType),
                method.Parameters.Select(parameter => GetTypeByName(parameter.Type)).ToArray()
            );
            var emitter = new Emitter(methodBuilder.GetILGenerator());
            emitter.FillMethodBody(method.Body);
        }

        private void CreateConstructor(TypeBuilder typeBuilder, ParsedConstructorInfo constructor)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard,
                constructor.Parameters.Select(parameter => GetTypeByName(parameter.Type)).ToArray()
            );
            var emitter = new Emitter(constructorBuilder.GetILGenerator());
            emitter.FillMethodBody(constructor.Body);
        }

        private TypeBuilder GetTypeBuilder(ParsedClassInfo classInfo)
        {
            if (classBuilders.TryGetValue(classInfo, out var builder))
            {
                return builder;
            }

            var assemblyName = new AssemblyName($"{_namespacePrefix}{classInfo.Name}");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(
                assemblyName.FullName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                GetParentTypeBuilder(classInfo.BaseClass)
            );

            classBuilders.Add(classInfo, typeBuilder);
            return typeBuilder;
        }

        private Type? GetParentTypeBuilder(ClassInfo? parentInfo)
        {
            if (parentInfo == null)
            {
                return null;
            }
            return parentInfo switch
            {
                ParsedClassInfo parsedClass => GetTypeBuilder(parsedClass),
                BuiltClassInfo builtClass => builtClass.Class,
                _ => throw new Exception($"Unknown ClassInfo type: {parentInfo}")
            };
        }

        private Type GetTypeByName(string name)
        {
            if (allClasses.TryGetValue(name, out var builtType))
            {
                return builtType;
            }

            var classInfo = ParsedClassInfo.GetByName(name);
            if (classInfo is ParsedClassInfo parsedClass && classBuilders.TryGetValue(parsedClass, out var builder))
            {
                return builder;
            }

            throw new Exception($"Can't find a type or type builder \"{name}\"");
        }
    }
}
