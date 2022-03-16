using System.Collections.Generic;
using System.Collections;
using System.Linq;

using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Analyze.Semantics
{
    internal class ClassTree : IEnumerable<ClassInfo>
    {

        public static ClassInfo RootClass = BuiltClassInfo.StandardClasses["Class"];

        private readonly Dictionary<string, ClassInfo> _knownClasses = new(BuiltClassInfo.StandardClasses);

        public ClassTree(Syntax.Tree syntaxTree)
        {
            foreach (var @class in syntaxTree)
            {
                var parsedClass = ParsedClassInfo.GetByClass(@class);
                _knownClasses.Add(parsedClass.Name, parsedClass);
                parsedClass.Context.AddClasses(this);
            }
            AddChildren(RootClass);
        }

        private void AddChildren(ClassInfo currentClassInfo)
        {
            var children = _knownClasses.Values.Where(c => c.BaseClass != null && c.BaseClass.Name == currentClassInfo.Name).ToList();
            currentClassInfo.DerivedClasses.AddRange(children);
            foreach (var childInfo in children)
            {
                AddChildren(childInfo);
            }
        }

        public ClassInfo? this[string name] => _knownClasses.GetValueOrDefault(name);
        public bool ClassExists(string name) => _knownClasses.ContainsKey(name);

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
