using System.Collections.Generic;
using OCompiler.Generics;

namespace OCompiler.Tokens
{
    internal class CodeEntity : TypeSafeEnum<string>
    {
        protected static List<CodeEntity> Entities { get; } = new();

        public static CodeEntity Empty { get; } = new("");
        protected CodeEntity(string literal) : base(literal) {
            Entities.Add(this);
        }

        public static CodeEntity GetByLiteral(string literal)
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
    }
}
