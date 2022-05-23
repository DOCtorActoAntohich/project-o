using OCompiler.Builtins.Primitives;

using System.Collections.Generic;

namespace OCompiler.Builtins.Compound;

public class List<T> : Class
{
    private readonly System.Collections.Generic.List<T> _list = new();

    public List(IEnumerable<T> items) => _list = new(items);

    public Integer Length() => new(_list.Count);
    
    public T Get(Integer index) => _list[index.Value];

    public void Set(Integer index, T item) => _list[index.Value] = item;
    
    public void Append(T item) => _list.Add(item);

    public void RemoveAt(Integer index) => _list.RemoveAt(index.Value);
    
    public T Pop() {
        var lastIndex = _list.Count - 1;
        var item = _list[lastIndex];
        _list.RemoveAt(lastIndex);
        return item;
    }
    
    public Integer Search(T item) => new(_list.IndexOf(item));
}
