using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.BooleanLiterals;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Exceptions;
using OCompiler.StandardLibrary.Type.Reference;
using OCompiler.StandardLibrary.Type.Value;
using Boolean = OCompiler.StandardLibrary.Type.Value.Boolean;
using Class = OCompiler.StandardLibrary.Type.Class;
using If = OCompiler.Analyze.Syntax.Declaration.Statement.If;
using Return = OCompiler.Analyze.Syntax.Declaration.Statement.Return;
using String = OCompiler.StandardLibrary.Type.Reference.String;
using Void = OCompiler.StandardLibrary.Type.Value.Void;
using While = OCompiler.Analyze.Syntax.Declaration.Statement.While;

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
    private Dictionary<string, Dictionary<string, FieldBuilder>> _fieldBuilders;
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
        _fieldBuilders = new Dictionary<string, Dictionary<string, FieldBuilder>>();
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
            
            // Build.
            _typeBuilders[@class.Name].CreateType();
        }
    }

    public Assembly Assembly => _assemblyBuilder;
    
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

    private FieldInfo? GetField(string @class, string name)
    {
        if (_standardTypes.ContainsKey(@class))
        {
            return _standardTypes[@class].GetField(name);
        }
        
        if (_fieldBuilders.ContainsKey(@class) && _fieldBuilders[@class].ContainsKey(name))
        {
            return _fieldBuilders[@class][name];
        }

        if (GetType(@class) is {BaseType: { } baseType})
        {
            return GetField(baseType.Name, name);
        }
        
        return null;
    }
    
    private MethodInfo? GetMethod(string @class, string name, Type[] parameters)
    {
        if (_standardTypes.ContainsKey(@class))
        {
            return _standardTypes[@class].GetMethod(
                name, 
                BindingFlags.Instance | BindingFlags.Public, parameters
            );
        }
        
        if (
            _methodBuilders.ContainsKey(@class) && 
            _methodBuilders[@class].ContainsKey(name) && 
            _methodBuilders[@class][name].ContainsKey(parameters)
        )
        {
            return _methodBuilders[@class][name][parameters];
        }
        
        if (GetType(@class) is {BaseType: {} baseType})
        {
            return GetMethod(baseType.Name, name, parameters);
        }

        return null;
    }

    private ConstructorInfo? GetConstructor(string @class, Type[] parameters)
    {
        if (_standardTypes.ContainsKey(@class))
        {
            return _standardTypes[@class].GetConstructor(parameters);
        }
        
        if (
            _constructorBuilders.ContainsKey(@class) &&
            _constructorBuilders[@class].ContainsKey(parameters)
        )
        {
            return _constructorBuilders[@class][parameters];
        }
        
        if (GetType(@class) is {BaseType: {} baseType})
        {
            return GetConstructor(baseType.Name, parameters);
        }

        return null;
    }
    
    private Type[] GetParameters(List<ParsedParameterInfo> parameters)
    {
        // Get types.
        var parameterTypes = new Type[parameters.Count];
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            parameterTypes[i] = GetType(parameters[i].Type) ?? throw new CompilationError(
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
        
        // Initialize field storage.
        if (!_fieldBuilders.ContainsKey(@class.Name))
        {
            _fieldBuilders.Add(@class.Name, new Dictionary<string, FieldBuilder>());
        }
    }
    
    private void DefineConstructor(ParsedConstructorInfo constructor)
    {
        // Get types.
        var parameterTypes = GetParameters(constructor.Parameters);
        
        // Check if type exists and has a builder.
        Type? type = GetType(constructor.Context.Class.Name);
        if (type is not TypeBuilder)
        {
            throw new CompilationError($"Defining constructor on invalid builder: {type}.");
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
        Type? type = GetType(method.Context.Class.Name);
        if (type is not TypeBuilder)
        {
            throw new CompilationError($"Defining method on invalid builder: {type}.");
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
                GetType(method.ReturnType) ?? throw new CompilationError($"Type not found: {method.ReturnType}"),
                parameterTypes
            )
        );
    }

    private void DefineField(ParsedFieldInfo field)
    {
        if (field.Type == null)
        {
            throw new CompilationError(
                $"Field {field.Name} in class {field.Context.Class.Name} has no Type."
            );
        }
        
        // Check if type exists and has a builder.
        Type? type = GetType(field.Context.Class.Name);
        if (type is not TypeBuilder)
        {
            throw new CompilationError($"Defining method on invalid builder: {type}.");
        }


        var fieldBuilder = ((TypeBuilder) type).DefineField(
            field.Name,
            GetType(field.Type) ?? throw new CompilationError($"Type not found: {field.Type}"),
            FieldAttributes.Public
        );
        
        _fieldBuilders[type.Name].Add(field.Name, fieldBuilder);

        foreach (var (parameters, builder) in _constructorBuilders[type.Name])
        {
            var scope = new Scope();
            var generator = builder.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0);
            // Push ref.
            scope.Push();
            
            // Load expression value to the stack.
            EmitExpression(type, null, null, generator, field.Expression.Expression, scope);
   
            // Store value in variable.
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            // Pop ref.
            scope.Pop();
            // Pop value.
            scope.Pop();
            
            while (scope.StackSize != 0)
            {
                scope.Pop();
                generator.Emit(OpCodes.Pop);
            }
        }
    }

    private void EmitConstructor(ParsedConstructorInfo constructor)
    {
        var parameterTypes = GetParameters(constructor.Parameters);
        var builder = _constructorBuilders[constructor.Context.Class.Name][parameterTypes];
        var generator = builder.GetILGenerator();

        var context = new Context(constructor.Context.Class, constructor);
        
        EmitBody(builder.DeclaringType!, context, generator, constructor.Body, new Scope());
    }
    
    private void EmitMethod(ParsedMethodInfo method)
    {   
        var parameterTypes = GetParameters(method.Parameters);
        var builder = _methodBuilders[method.Context.Class.Name][method.Name][parameterTypes];
        var generator = builder.GetILGenerator();

        var context = new Context(method.Context.Class, method);
        
        EmitBody(builder.DeclaringType!, context, generator, method.Body, new Scope());
    }

    private void EmitBody(Type type, Context context, ILGenerator generator, Body body, Scope scope)
    {
        var childScope = scope.Child;
        
        foreach (var statement in body)
        {
            Emit(type, context, generator, statement, childScope);
        }

        while (childScope.StackSize != 0)
        {
            childScope.Pop();
            generator.Emit(OpCodes.Pop);
        }
    }

    private void Emit(Type type, Context context, ILGenerator generator, IBodyStatement statement, Scope scope)
    {
        switch (statement)
        {
            case Variable variable:
                // Estimate expression.
                var variableInfo = new ExpressionInfo(variable.Expression, context);
                
                // Get builder.
                var variableBuilder = generator.DeclareLocal(
                    GetType(variableInfo.Type) ?? throw new CompilationError(
                        $"Type not found: {variable.Identifier.Literal}"
                    )
                );
                
                // Load expression value to the stack.
                EmitExpression(type, null, context, generator, variable.Expression, scope);
                
                // Store value in variable.
                generator.Emit(OpCodes.Stloc, variableBuilder);
                // Pop value.
                scope.Pop();
                
                // Save to scope.
                scope.Locals.Add(variable.Identifier.Literal, variableBuilder);
                break;
            case Assignment assignment:
                // Assign to the field.
                if (assignment.Variable.Token is This)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    // Push ref.
                    scope.Push();
                    
                    // Load expression value to the stack.
                    EmitExpression(type, null, context, generator, assignment.Value, scope);

                    // Assignment to local variable.
                    generator.Emit(
                        OpCodes.Stfld,
                        GetField(
                            type.Name, 
                            assignment.Variable.Child!.Token.Literal
                        ) ?? throw new CompilationError(
                            $"Type not found: {assignment.Variable.Child!.Token.Literal}"
                        )
                    );
                    // Pop ref.
                    scope.Pop();
                    // Pop value.
                    scope.Pop();
                    break;
                }
                // Load expression value to the stack.
                EmitExpression(type, null, context, generator, assignment.Value, scope);

                // Assignment to local variable.
                generator.Emit(OpCodes.Stloc, scope.Get(assignment.Variable.Token.Literal)!);
                // Pop value.
                scope.Pop();
                break;
            case If conditional:
                Label @else = generator.DefineLabel();
                Label end = generator.DefineLabel();
                
                // Emit condition.
                EmitExpression(type, null, context, generator, conditional.Condition, scope);
                // Access real value.
                generator.Emit(OpCodes.Ldfld, typeof(Boolean).GetField("_value")!);
                
                // Check.
                generator.Emit(OpCodes.Brfalse, @else);
                scope.Pop();
                
                // If.
                EmitBody(type, context, generator, conditional.Body, scope);
                generator.Emit(OpCodes.Br, end);
                
                // Else.
                generator.MarkLabel(@else);
                EmitBody(type, context, generator, conditional.ElseBody, scope);
                
                // End.
                generator.MarkLabel(end);
                
                break;
            case While loop:
                Label begin = generator.DefineLabel();
                Label exit = generator.DefineLabel();
                
                // Begin.
                generator.MarkLabel(begin);
                
                // Emit condition.
                EmitExpression(type, null, context, generator, loop.Condition, scope);
                // Access real value.
                generator.Emit(OpCodes.Ldfld, typeof(Boolean).GetField("_value")!);
                
                // Check.
                generator.Emit(OpCodes.Brfalse, exit);
                scope.Pop();
                
                EmitBody(type, context, generator, loop.Body, scope);
                generator.Emit(OpCodes.Br, begin);
                
                // Exit.
                generator.MarkLabel(exit);
                
                break;
            case Return @return:
                if (@return.ReturnValue is not null)
                {
                    // Load expression value to the stack.
                    EmitExpression(type, null, context, generator, @return.ReturnValue, scope);
                }
                else
                {
                    //Load Void value to the stack.
                    EmitExpression(
                        type, 
                        null, 
                        context, 
                        generator,
                        new Call(new Identifier("Void"), new List<Expression>()), 
                        scope
                    );
                }

                if (context.Callable is ParsedConstructorInfo)
                {
                    // No return value on constructors.
                    generator.Emit(OpCodes.Pop);
                }
                
                generator.Emit(OpCodes.Ret);
                scope.Pop();
                break;
            case Expression expression:
                EmitExpression(type, null, context, generator, expression, scope);
                break;
            
            default:
                throw new CompilationError($"Unknown IBodyStatement: {statement}");
        }
    }

    private void EmitExpression(
        Type type, 
        Type? currentType, 
        Context context,
        ILGenerator generator, 
        Expression expression, 
        Scope scope
    )
    {
        // Method or constructor call.
        if (expression is Call call)
        {
            // var typeName = call.Token.Literal;
            if (call.Token is Base)
            {
                if (currentType is not null)
                {
                    throw new CompilationError("Current type must be null.");
                }
                
                if (type.BaseType is null)
                {
                    throw new CompilationError("No class to inherit from.");
                }
                    
                // Load 'this' reference.
                generator.Emit(OpCodes.Ldarg_0);
                // Push ref.
                scope.Push();
            }
            
            // Estimate all callable types.
            var parameterTypes = new List<Type>();
            foreach (var argument in call.Arguments)
            {
                // Load parameter into stack.
                EmitExpression(type, null, context, generator, argument, scope);

                // Estimate type.
                var argumentInfo = new ExpressionInfo(argument, context);
                parameterTypes.Add(
                    GetType(argumentInfo.Type) ?? throw new CompilationError(
                        $"Type not found: {argumentInfo.Type}"
                    )
                );
            }
            
            // Inheritance.
            if (call.Token is Base)
            {
                generator.Emit(
                    OpCodes.Call, 
                    GetConstructor(type.BaseType!.Name, parameterTypes.ToArray()) ?? throw new CompilationError(
                        $"Constructor not found: {parameterTypes}"
                    )
                );
                
                // Pop ref.
                scope.Pop();
            }
            // Construction call.
            else if (currentType is null)
            {
                // Type of object that will be created.
                currentType = GetType(call.Token.Literal) ?? throw new CompilationError(
                    $"Type not found: {call.Token.Literal}"
                );
                
                // Create new object.
                generator.Emit(
                    OpCodes.Newobj, 
                    GetConstructor(currentType.Name, parameterTypes.ToArray()) ?? throw new CompilationError(
                        $"Constructor not found: {parameterTypes}"
                    )
                );

                // Push new obj.
                scope.Push();
            }
            else
            {
                var method = GetMethod(
                    currentType.Name, 
                    call.Token.Literal, 
                    parameterTypes.ToArray()
                ) ?? throw new CompilationError(
                    $"Method not found: {parameterTypes.ToArray()}"
                );
                currentType = method.ReturnType;
                generator.Emit(OpCodes.Callvirt, method);
                
                // Pop ref.
                scope.Pop();
                // Push result.
                scope.Push();

                // Return void.
                if (currentType == typeof(Void))
                {
                    generator.Emit(OpCodes.Pop);
                    scope.Pop();
                }
            }
            
            // Parameters are poped by method or construction.
            for (int i = 0; i < parameterTypes.Count; i++)
            {
                // Pop arg.
                scope.Pop();
            }
        }
        // Literal or field load.
        else
        { 
            switch (expression.Token)
            {
                case StringLiteral stringLiteral:
                    currentType = typeof(String);
                    generator.Emit(OpCodes.Ldstr, stringLiteral.Literal);
                    generator.Emit(OpCodes.Newobj, GetConstructor(currentType.Name, new [] {typeof(string)})!);
                    // Push value.
                    scope.Push();
                    break;
                
                case RealLiteral realLiteral:
                    currentType = typeof(Real);
                    generator.Emit(OpCodes.Ldc_R8, realLiteral.Value);
                    generator.Emit(OpCodes.Newobj, GetConstructor(currentType.Name,new [] {typeof(double)})!);
                    // Push value.
                    scope.Push();
                    break;
                
                case IntegerLiteral integerLiteral:
                    currentType = typeof(Integer);
                    generator.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    generator.Emit(OpCodes.Newobj, GetConstructor(currentType.Name,new [] {typeof(int)})!);
                    // Push value.
                    scope.Push();
                    break;
                
                case True:
                    currentType = typeof(Boolean);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Newobj, GetConstructor(currentType.Name,new [] {typeof(bool)})!
                    );
                    // Push value.
                    scope.Push();
                    break;
                
                case False:
                    currentType = typeof(Boolean);
                    generator.Emit(OpCodes.Ldc_I4_0);
                    generator.Emit(OpCodes.Newobj, GetConstructor(currentType.Name,new [] {typeof(bool)})!
                    );
                    // Push value.
                    scope.Push();
                    break;
                
                // Accessing this.
                case This:
                    if (currentType is not null)
                    {
                        throw new CompilationError($"Current type should be null: {currentType}");
                    }
                    
                    currentType = type;
                    // Load 'this' reference.
                    generator.Emit(OpCodes.Ldarg_0);
                    // Push value.
                    scope.Push();
                    break;
                
                // Call constructor.
                case Identifier name when currentType is null && GetType(name.Literal) is not null:
                    EmitExpression(
                        type, 
                        currentType, 
                        context, 
                        generator, 
                        new Call(
                            name, 
                            new List<Expression>(), 
                            null, 
                            expression.Child
                        ), 
                        scope
                    );
                    return;

                // Accessing local variable.
                case Identifier name when currentType is null && scope.Get(name.Literal) is not null:
                    currentType = scope.Get(name.Literal)!.LocalType;
                    // Load reference to the local variable.
                    generator.Emit(OpCodes.Ldloc, scope.Get(name.Literal)!);
                    // Push value.
                    scope.Push();
                    break;
                
                // Accessing argument.
                case Identifier name when currentType is null:
                    var index = 0;
                    foreach (var parameter in context.Callable!.Parameters)
                    {
                        if (parameter.Name == name.Literal)
                        {
                            break;
                        }

                        index++;
                    }

                    if (index == context.Callable!.Parameters.Count)
                    {
                        throw new CompilationError($"Cannot find argument {name.Literal}");
                    }

                    var parameterInfo = context.Callable!.Parameters[index];
                    
                    currentType = GetType(parameterInfo.Type) ?? throw new CompilationError(
                        $"Parameter not found: {name.Literal}"
                    );
                    // Load reference to the local variable.
                    generator.Emit(OpCodes.Ldarg, index + 1);
                    // Push value.
                    scope.Push();
                    break;
                
                // Accessing field of field.
                case Identifier name:
                    // Get field.
                    var field = GetField(currentType.Name, name.Literal) ?? throw new CompilationError(
                        $"Field not found: {name.Literal}"
                    );

                    currentType = field.FieldType;
                    // Load reference to the field.
                    generator.Emit(OpCodes.Ldfld, field);
                    
                    // Pop ref.
                    scope.Pop();
                    // Push value.
                    scope.Push();
                    
                    break;
                
                default:
                    throw new CompilationError($"Invalid expression: {expression}");
            }
        }

        // Recursion.
        if (expression.Child is not null)
        {
            EmitExpression(type, currentType, context, generator, expression.Child, scope);
        }
    }
}
