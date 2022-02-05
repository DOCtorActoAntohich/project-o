using System.Collections.Generic;
using OCompiler.Generics;

namespace OCompiler.Analyze.Lexical.Literals
{
    internal class ReservedLiteral : TypeSafeEnum<string>
    {
        protected static List<ReservedLiteral> Entities { get; } = new();

        public static ReservedLiteral Empty { get; } = new("");

        protected ReservedLiteral(string literal) : base(literal) {
            Entities.Add(this);
        }

        public static ReservedLiteral GetByValue(string literal)
        {
            foreach (var entity in Entities)
            {
                if (entity.Value == literal)
                {
                    return entity;
                }
            }
            return Empty;
        }

        static ReservedLiteral()
        {
            // This is required to properly instantiate subclasses
            // so that they are added to Entities list
            Keyword.Loop.ToString();
            Delimiter.Dot.ToString();
            Boolean.True.ToString();
        }
    }
}
