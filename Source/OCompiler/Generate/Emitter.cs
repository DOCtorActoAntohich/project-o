using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.StandardLibrary.Type.Reference;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OCompiler.Generate
{
    internal class Emitter
    {
        private ILGenerator IlGenerator { get; }

        public Emitter(ILGenerator ilGenerator)
        {
            IlGenerator = ilGenerator;
        }

        public void FillMethodBody(Body methodBody)
        {
            foreach (var statement in methodBody)
            {
                Emit(statement);
            }
            IlGenerator.Emit(OpCodes.Ret);
        }

        private void Emit(IBodyStatement statement)
        {
            switch (statement)
            {
                case Variable variable:
                    // TODO
                    break;
                case Assignment assignment:
                    // TODO
                    break;
                case If conditional:
                    // TODO
                    break;
                case Return @return:
                    // TODO
                    break;
                case While loop:
                    // TODO
                    break;
                case Expression expression:
                    // TODO
                    break;
                default:
                    throw new Exception($"Unknown IBodyStatement: {statement}");
            }
        }
    }
}
