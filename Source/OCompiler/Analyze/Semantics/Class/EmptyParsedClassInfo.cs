using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCompiler.Analyze.Semantics.Class
{
    internal class EmptyParsedClassInfo : ParsedClassInfo
    {
        public EmptyParsedClassInfo(string name)
        {
            Name = name;
        }
    }
}
