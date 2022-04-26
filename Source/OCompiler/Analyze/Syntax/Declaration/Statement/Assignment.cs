using System;
using OCompiler.Analyze.Lexical.Tokens.Delimiters;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Assignment : IStatement
{
    public Expression.Expression Variable { get; }
    
    public Expression.Expression Value { get; }
    
    public static bool TryParse(TokenEnumerator tokens, out Assignment? assignment)
    {
        int index = tokens.Index;
        
        // Variable.
        if (!Expression.Expression.TryParse(tokens, out Expression.Expression? variable))
        {
            assignment = null;
            return false;
        }
        
        // Assign delimiter.
        if (tokens.Current() is not Lexical.Tokens.Delimiters.Equals)
        {
            tokens.RestoreIndex(index);
            assignment = null;
            return false;
        }

        // Get next token.
        tokens.Next();
        
        // Expression.
        if (!Expression.Expression.TryParse(tokens, out Expression.Expression? value))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected expression");
        }
        
        assignment = new Assignment(variable!, value!);
        return true;
    }
    
    protected Assignment(Expression.Expression variable, Expression.Expression value)
    {
        Variable = variable;
        Value = value;
    }
    
    public string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"{Variable} := {Value}";
    }
}
