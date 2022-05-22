using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.SemanticsV2.Tree;
using ClassDict = Dictionary<string, ClassDeclaration>;

internal class InheritanceTree
{
    public static readonly string RootClassName = "Class";

    public bool IsValid { get; private set; }
    
    
    private readonly ClassDict _classes;
    private readonly Dictionary<string, int> _inheritanceDepth = new();

    
    public InheritanceTree(ClassDict builtins, ClassDict parsedClasses)
    {
        var allClasses =
            builtins.GetEnumerator().MakeEnumerable().Concat(parsedClasses.GetEnumerator().MakeEnumerable());

        _classes = new ClassDict(allClasses);

        ValidateTree();
    }

    private void ValidateTree()
    {
        _inheritanceDepth.Add(RootClassName, 0);
        
        AddChildrenToInheritanceDepthTree(RootClassName);
        CheckForUntouchedClasses();
        
        IsValid = true;
    }

    private void AddChildrenToInheritanceDepthTree(string @class)
    {
        var parentDepth = _inheritanceDepth[@class];
        var currentDepth = parentDepth + 1;

        var newParents = new List<string>();
        
        foreach (var potentialChild in _classes.Values)
        {
            if (potentialChild.BaseType == null || potentialChild.BaseType.Name != @class)
            {
                continue;
            }
            
            _inheritanceDepth.Add(potentialChild.Name, currentDepth);
            newParents.Add(potentialChild.Name);
        }

        foreach (var parent in newParents)
        {
            AddChildrenToInheritanceDepthTree(parent);
        }
    }

    private void CheckForUntouchedClasses()
    {
        if (_classes.Count == _inheritanceDepth.Count)
        {
            return;
        }

        var untouchedClasses = _classes.Keys.Where(name => !_inheritanceDepth.ContainsKey(name));
        var errorMessage = new StringBuilder("All or some of the following classes form a cycle in inheritance tree: ");
        errorMessage.Append(string.Join(", ", untouchedClasses.ToArray()));

        throw new AnalyzeError(errorMessage.ToString());
    }
}