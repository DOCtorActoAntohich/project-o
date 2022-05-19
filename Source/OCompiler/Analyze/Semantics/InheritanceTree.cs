using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Analyze.Semantics;

internal class InheritanceTree : IEnumerable<ClassInfo>
{
    private static readonly ClassInfo RootClass = BuiltClassInfo.StandardClasses["Class"];
    
    public Dictionary<string, ClassInfo> TraversedClasses { get; } = new();

    
    public InheritanceTree(Syntax.Tree syntaxTree)
    {
        var classesToTraverse = GatherAvailableClasses(syntaxTree);

        var orphanClasses = AddChildrenTo(RootClass, classesToTraverse);
        if (orphanClasses.Count == 0)
        {
            return;
        }
        
        
        var orphan = classesToTraverse.First();
        if (orphan.BaseClass == null)
        {
            throw new System.Exception($"Class {orphan.Name} does not have a base class");
        }
        throw new System.Exception($"Class {orphan.BaseClass.Name} (base of {orphan.Name}) does not exist");
    }

    
    private static List<ClassInfo> GatherAvailableClasses(Syntax.Tree syntaxTree)
    {
        var classesToTraverse = new List<ClassInfo>(BuiltClassInfo.StandardClasses.Values);
        foreach (var @class in syntaxTree)
        {
            var parsedClass = ParsedClassInfo.GetByClass(@class);
            classesToTraverse.Add(parsedClass);
        }

        return classesToTraverse;
    }

    private List<ClassInfo> AddChildrenTo(ClassInfo parent, List<ClassInfo> untouchedClasses)
    {
        var currentChildren = untouchedClasses.Where(
            classInfo => classInfo.BaseClass != null && classInfo.BaseClass.Name == parent.Name).ToList();

        AddDirectChildren(parent, currentChildren);
        MarkTraversed(parent);
        untouchedClasses.Remove(parent);

        foreach (var child in currentChildren)
        {
            AddChildrenTo(child, untouchedClasses);
        }
        
        return untouchedClasses;
    }

    private static void AddDirectChildren(ClassInfo parent, IEnumerable<ClassInfo> children)
    {
        parent.DerivedClasses.AddRange(children);
    }
    
    private void MarkTraversed(ClassInfo @class)
    {
        TraversedClasses.Add(@class.Name, @class);
    }


    private IEnumerable<ClassInfo> DfsTraverseBranchesOf(ClassInfo currentClass)
    {
        IEnumerable<ClassInfo> allChildren = new[] {currentClass};
        foreach (var child in currentClass.DerivedClasses)
        {
            var branch = DfsTraverseBranchesOf(child);
            allChildren = allChildren.Concat(branch);
        }
        
        foreach (var child in allChildren)
        {
            yield return child;
        }
    }
    
    public IEnumerator<ClassInfo> GetEnumerator()
    {
        return DfsTraverseBranchesOf(RootClass).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}