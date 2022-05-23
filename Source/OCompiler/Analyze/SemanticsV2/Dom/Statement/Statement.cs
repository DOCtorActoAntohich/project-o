using System;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement;

internal abstract class Statement : CodeObject
{
    public ICanHaveStatements? Holder { get; set; }
    
    public Statement() : base("")
    {
    }

    public string ToString(string prefix = "", string nestedPrefix = "")
    {
        return this switch
        {
            ConditionStatement condition => condition.ToString(prefix, nestedPrefix),
            LoopStatement loop => loop.ToString(prefix, nestedPrefix),
            AssignStatement assignment => assignment.ToString(prefix),
            ExpressionStatement singleExpression => singleExpression.ToString(prefix),
            ReturnStatement @return => @return.ToString(prefix),
            VariableDeclarationStatement variable => variable.ToString(prefix),
            _ => throw new CompilerInternalError("Unknown statement type")
        };
    }
}