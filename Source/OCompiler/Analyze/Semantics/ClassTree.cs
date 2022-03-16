using System.Collections.Generic;
using System.Collections;
using System.Linq;

using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Analyze.Semantics
{
    internal class ClassTree : IEnumerable<ClassInfo>
    {

        public static ClassInfo RootClass = BuiltClassInfo.StandardClasses["Class"];
        public static Dictionary<string, ClassInfo> TraversedClasses { get; } = new();

        public ClassTree(Syntax.Tree syntaxTree)
        {
            var classesToTraverse = new List<ClassInfo>(BuiltClassInfo.StandardClasses.Values);

            foreach (var @class in syntaxTree)
            {
                var parsedClass = ParsedClassInfo.GetByClass(@class);
                classesToTraverse.Add(parsedClass);
            }

            AddChildren(RootClass, classesToTraverse);
            if (classesToTraverse.Count > 0)
            {
                var orphanClass = classesToTraverse.First();
                if (orphanClass.BaseClass == null)
                {
                    throw new System.Exception($"Class {orphanClass.Name} does not have a base class");
                }
                throw new System.Exception($"Class {orphanClass.BaseClass.Name} (base of {orphanClass.Name}) does not exist");
            }
        }

        private void AddChildren(ClassInfo currentClassInfo, List<ClassInfo> remainingClasses)
        {
            var children = remainingClasses.Where(c => c.BaseClass != null && c.BaseClass.Name == currentClassInfo.Name).ToList();
            currentClassInfo.DerivedClasses.AddRange(children);
            TraversedClasses.Add(currentClassInfo.Name, currentClassInfo);
            remainingClasses.Remove(currentClassInfo);
            foreach (var childInfo in children)
            {
                AddChildren(childInfo, remainingClasses);
            }
        }

        public ClassInfo? this[string name] => TraversedClasses.GetValueOrDefault(name);
        public static bool ClassExists(string name) => TraversedClasses.ContainsKey(name);

        public IEnumerable<ClassInfo> TraverseTree(ClassInfo currentClassInfo)
        {
            // DFS class tree traversal
            yield return currentClassInfo;
            foreach (var childInfo in currentClassInfo.DerivedClasses)
            {
                foreach (var classInfo in TraverseTree(childInfo))
                {
                    yield return classInfo;
                }
            }
        }

        public IEnumerator<ClassInfo> GetEnumerator()
        {
            return TraverseTree(RootClass).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return TraverseTree(RootClass).GetEnumerator();
        }
    }
}
