using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using OCompiler.Analyze.SemanticsV2;
using OCompiler.Analyze.SemanticsV2.Dom;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Builtins.Primitives;
using OCompiler.Exceptions;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
using String = OCompiler.Builtins.Primitives.String;
using Void = OCompiler.Builtins.Primitives.Void;

namespace OCompiler.Generate;


internal class EmitterV2
{
    private readonly AnnotatedSyntaxTreeV2 _tree;
    private readonly AssemblyBuilder _assemblyBuilder;
    private readonly ModuleBuilder _moduleBuilder;

    public Assembly Assembly => _assemblyBuilder;
    
    public EmitterV2(AnnotatedSyntaxTreeV2 tree)
    {
        _tree = tree;
        
        // Define builders.
        _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("OCompiler"), AssemblyBuilderAccess.Run);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Runtime");
        
        // Define types.
        foreach (var @class in tree.ParsedClasses.Values)
        {   
            DefineType(@class);
        }
        
        // Define methods, constructor, fields.
        foreach (var @class in tree.ParsedClasses.Values)
        {
            foreach (var method in @class.Methods)
            {
                DefineMethod(method);
            }
            
            foreach (var constructor in @class.Constructors)
            {
                DefineConstructor(constructor);
            }
            
            foreach (var field in @class.Fields)
            {
                DefineField(field);
            }
        }
        
        // Emit methods and constructor.
        foreach (var @class in tree.ParsedClasses.Values)
        {
            foreach (var method in @class.Methods)
            {
                EmitMethod(method);
            }

            foreach (var constructor in @class.Constructors)
            {
                EmitConstructor(constructor);
            }

            ((TypeBuilder) @class.DotnetType!).CreateType();
        }
    }
    
    private void ResolveReference(TypeReference reference)
    {
        if (reference.DotnetType is not null)
        {
            return;
        }
        
        // Resolve all parents.
        foreach (var genericType in reference.GenericTypes)
        {
            DefineType(_tree.GetClass(genericType.Name));
            ResolveReference(genericType);
        }
        
        reference.DotnetType = _tree.GetClass(reference.Name).DotnetType;
    }
    
    private void DefineType(ClassDeclaration @class)
    {
        // Already defined.
        if (@class.DotnetType is not null)
        {
            return;
        }
        
        // Make sure parents are defined.
        ResolveReference(@class.BaseType!);

        // Create builder.
        @class.DotnetType = _moduleBuilder.DefineType(
            @class.Name,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            (Type)@class.BaseType!.DotnetType!
        );
    }
    
    private void DefineMethod(MemberMethod method)
    {
        if (method.DotnetType is not null)
        {
            return;
        }
        
        ResolveReference(method.ReturnType);
        foreach (var parameter in method.Parameters)
        {
            ResolveReference(parameter.Type);
        }
        
        method.DotnetType = ((TypeBuilder) method.Owner!.DotnetType!).DefineMethod(
            method.Name, MethodAttributes.Public, CallingConventions.HasThis,
            (Type) method.ReturnType.DotnetType!, 
            method.Parameters.Select(parameter => (Type) parameter.Type.DotnetType!).ToArray());
    }
    
    private void DefineConstructor(MemberConstructor constructor)
    {
        if (constructor.DotnetType is not null)
        {
            return;
        }
        
        foreach (var parameter in constructor.Parameters)
        {
            ResolveReference(parameter.Type);
        }
        
        constructor.DotnetType = ((TypeBuilder) constructor.Owner!.DotnetType!).DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.HasThis,
            constructor.Parameters.Select(parameter => (Type) parameter.Type.DotnetType!).ToArray());
    }
    
    private void DefineField(MemberField field)
    {
        if (field.DotnetType is not null)
        {
            return;
        }

        ResolveReference(field.Type);
        
        field.DotnetType = ((TypeBuilder) field.Owner!.DotnetType!).DefineField(
            field.Name, (Type) field.Type.DotnetType!, FieldAttributes.Public);
    }
    
    // typeof(Dictionary<,>).MakeGenericType(int, string)

    private void EmitMethod(MemberMethod method)
    {
        EmitBody(((MethodBuilder) method.DotnetType!).GetILGenerator(), method.Statements, new ScopeV2());
    }

    private void EmitConstructor(MemberConstructor constructor)
    {
        EmitBody(((ConstructorBuilder) constructor.DotnetType!).GetILGenerator(), constructor.Statements, 
            new ScopeV2());   
    }
    
    private void EmitBody(ILGenerator generator, StatementsCollection statements, ScopeV2 parentScope)
    {
        if (statements.Count == 0)
        {
            return;
        }
        
        var scope = parentScope.GetChild();
        foreach (var statement in statements)
        {
            EmitStatement(generator, statement, scope);
        }
        
        if (scope.StackSize != 0)
        {
            throw new CompilerInternalError("Stack is not empty.");
        }
    }

    private void EmitStatement(ILGenerator generator, Statement statement, ScopeV2 scope)
    {
        switch (statement)
        {
            case AssignStatement {LValue: VariableReferenceExpression} assignStatement:
                EmitExpression(generator, assignStatement.RValue, scope);

                generator.Emit(OpCodes.Stloc, scope.GetVariable(assignStatement.LValue.Name));
                scope.DecreaseStackSize();
                break;
            
            case AssignStatement {LValue: FieldReferenceExpression} assignStatement:
                EmitExpression(generator, ((FieldReferenceExpression) assignStatement.LValue).SourceObject, scope);
                EmitExpression(generator, assignStatement.RValue, scope);
  
                generator.Emit(OpCodes.Stfld,
                    (FieldBuilder) ((FieldReferenceExpression) assignStatement.LValue).Field.DotnetType!);
                scope.DecreaseStackSize();
                scope.DecreaseStackSize();
                break;

            case VariableDeclarationStatement variableDeclarationStatement:
                ResolveReference(variableDeclarationStatement.Type);
                var variable = generator.DeclareLocal((Type) variableDeclarationStatement.Type.DotnetType!);
                scope.SetVariable(variableDeclarationStatement.Name, variable);
                
                EmitExpression(generator, variableDeclarationStatement.InitExpression, scope);

                generator.Emit(OpCodes.Stloc, variable);
                scope.DecreaseStackSize();
                break;
            
            case ExpressionStatement expressionStatement:
                EmitExpression(generator, expressionStatement.Expression, scope);
                break;
            
            case ReturnStatement returnStatement:
                if (GetTypeMember(returnStatement) is not MemberConstructor)
                {
                    EmitExpression(generator, returnStatement.Expression, scope);
                    scope.DecreaseStackSize();
                }
                
                generator.Emit(OpCodes.Ret);
                break;
            
            case ConditionStatement conditionStatement:
                Label @else = generator.DefineLabel();
                Label end = generator.DefineLabel();
                
                // Emit condition.
                EmitExpression(generator, conditionStatement.Condition, scope);
                // Access real value.
                generator.Emit(OpCodes.Ldfld, typeof(Boolean).GetField("_value")!);
                
                // Check.
                generator.Emit(OpCodes.Brfalse, @else);
                scope.DecreaseStackSize();
                
                // If.
                EmitBody(generator, conditionStatement.Statements, scope);
                generator.Emit(OpCodes.Br, end);
                
                // Else.
                generator.MarkLabel(@else);
                EmitBody(generator, conditionStatement.ElseStatements, scope);
                
                // End.
                generator.MarkLabel(end);
                break;
            
            case LoopStatement loopStatement:
                Label begin = generator.DefineLabel();
                Label exit = generator.DefineLabel();
                
                // Begin.
                generator.MarkLabel(begin);
                
                // Emit condition.
                EmitExpression(generator, loopStatement.Condition, scope);
                // Access real value.
                generator.Emit(OpCodes.Ldfld, typeof(Boolean).GetField("_value")!);
                
                // Check.
                generator.Emit(OpCodes.Brfalse, exit);
                scope.DecreaseStackSize();
                
                EmitBody(generator, loopStatement.Statements, scope);
                generator.Emit(OpCodes.Br, begin);
                
                // Exit.
                generator.MarkLabel(exit);
                break;
            
            default:
                throw new CompilerInternalError("Cannot determine statement type.");
        }
        
        // Statement value is unused.
        if (scope.StackSize != 0)
        {
            generator.Emit(OpCodes.Pop);
            scope.DecreaseStackSize();
        }
    }
    
    private void EmitExpression(ILGenerator generator, Expression expression, ScopeV2 scope)
    {
        switch (expression)
        {
            case ThisReferenceExpression:
                generator.Emit(OpCodes.Ldarg_0); 
                scope.IncreaseStackSize();
                break;
            
            case VariableReferenceExpression variableReferenceExpression:
                if (IndexOfParameter(variableReferenceExpression) is { } indexOfParameter)
                {
                    generator.Emit(OpCodes.Ldarg, indexOfParameter);
                }
                else
                {
                    generator.Emit(OpCodes.Ldloc, scope.GetVariable(variableReferenceExpression.Name));
                }
                
                scope.IncreaseStackSize();
                break;
            
            case FieldReferenceExpression fieldReferenceExpression:
                EmitExpression(generator, fieldReferenceExpression.SourceObject, scope);
                generator.Emit(OpCodes.Ldfld, (FieldBuilder) fieldReferenceExpression.Field.DotnetType!);
                break;

            case BaseConstructorCallExpression baseConstructorCallExpression:
                generator.Emit(OpCodes.Ldarg_0);
                
                foreach (var argument in baseConstructorCallExpression.Arguments)
                {
                    EmitExpression(generator, argument, scope);
                    scope.DecreaseStackSize();
                }

                generator.Emit(OpCodes.Call, 
                    (ConstructorInfo) baseConstructorCallExpression.Constructor.DotnetType!);
                break;
            
            case MethodCallExpression methodCallExpression:
                EmitExpression(generator, methodCallExpression.SourceObject, scope);
                scope.DecreaseStackSize();
                
                foreach (var argument in methodCallExpression.Arguments)
                {
                    EmitExpression(generator, argument, scope);
                    scope.DecreaseStackSize();
                }
                
                generator.Emit(OpCodes.Callvirt, (MethodInfo) methodCallExpression.Method.DotnetType!);
                scope.IncreaseStackSize();
                
                if (methodCallExpression.Method.ReturnType.Name == nameof(Void))
                {
                    generator.Emit(OpCodes.Pop);
                    scope.DecreaseStackSize();
                }
                
                break;
            
            case ObjectCreateExpression objectCreateExpression:
                foreach (var argument in objectCreateExpression.Arguments)
                {
                    EmitExpression(generator, argument, scope);
                    scope.DecreaseStackSize();
                }
                
                generator.Emit(OpCodes.Newobj, 
                    (ConstructorInfo) objectCreateExpression.Constructor.DotnetType!);
                scope.IncreaseStackSize();
                break;
            
            case BooleanLiteralExpression booleanLiteralExpression:
                generator.Emit(booleanLiteralExpression.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Newobj, typeof(Boolean).GetConstructor(new []{typeof(bool)})!);
                scope.IncreaseStackSize();
                break;
            
            case IntegerLiteralExpression integerLiteralExpression:
                generator.Emit(OpCodes.Ldc_I4, integerLiteralExpression.Value);
                generator.Emit(OpCodes.Newobj, typeof(Integer).GetConstructor(new []{typeof(int)})!);
                scope.IncreaseStackSize();
                break;
            
            case RealLiteralExpression realLiteralExpression:
                generator.Emit(OpCodes.Ldc_R8, realLiteralExpression.Value);
                generator.Emit(OpCodes.Newobj, typeof(Real).GetConstructor(new []{typeof(double)})!);
                scope.IncreaseStackSize();
                break;
            
            case StringLiteralExpression stringLiteralExpression:
                generator.Emit(OpCodes.Ldstr, stringLiteralExpression.Value);
                generator.Emit(OpCodes.Newobj, typeof(String).GetConstructor(new []{typeof(string)})!);
                scope.IncreaseStackSize();
                break;
            
            default:
                throw new CompilerInternalError("Cannot identify holder of expression.");
        }
    }

    private int? IndexOfParameter(VariableReferenceExpression expression)
    {
        var index = ((CallableMember) GetTypeMember(expression.ParentStatement)).Parameters.IndexOf(expression.Name);

        if (index == -1)
        {
            return null;
        }
        
        return index + 1;
    }
    
    private TypeMember GetTypeMember(Statement statement)
    {
        if (statement.Holder is TypeMember member)
        {
            return member;
        }
        
        if (statement.Holder is Statement holder)
        {
            return GetTypeMember(holder);
        }
        
        throw new CompilerInternalError("Cannot identify holder of statement.");
    }
}