namespace OCompiler.Generics
{
    class TypeSafeEnum<T>
    {
        public T Value { get; }

        protected TypeSafeEnum(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name} {{{Value}}}";
        }
    }
}
