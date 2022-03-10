using System.Collections.Generic;

namespace OCompiler.Utils
{
    internal class BufferedEnumerator<T>
    {
        private readonly List<T> _buffer = new();
        private readonly IEnumerator<T> _enumerator;
        private int _tailIndex = 1;
        public T Current { get; private set; }

        public BufferedEnumerator(IEnumerable<T> items)
        {
            _enumerator = items.GetEnumerator();
            _enumerator.MoveNext();
            Current = _enumerator.Current;
        }

        public bool MoveNext()
        {
            if (_tailIndex > 1)
            {
                --_tailIndex;
                Current = _buffer[^_tailIndex];
                return true;
            }
            if (!_enumerator.MoveNext())
            {
                return false;
            }

            Current = _enumerator.Current;
            _buffer.Add(Current);
            return true;
        }

        public bool MoveBack()
        {
            if (_tailIndex >= _buffer.Count)
            {
                return false;
            }

            ++_tailIndex;
            Current = _buffer[^_tailIndex];
            return true;
        }
    }
}
