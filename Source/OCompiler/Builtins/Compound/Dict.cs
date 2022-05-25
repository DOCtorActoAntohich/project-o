using System.Collections.Generic;

using OCompiler.Builtins.Primitives;

namespace OCompiler.Builtins.Compound;

public class Dict<K, V> : Class where K : notnull
{
    private readonly Dictionary<K, V> _dictionary;

    public Dict() => _dictionary = new();

    public Dict(IEnumerable<KeyValuePair<K, V>> items) => _dictionary = new(items);

    public Integer Length() => new(_dictionary.Count);
    public V Get(K key) => _dictionary[key];

    public void Set(K key, V value) => _dictionary[key] = value;

    public void Remove(K key) => _dictionary.Remove(key);
    
    public K Search(V value)
    {
        var isValueNull = value is null;

        foreach (var (k, v) in _dictionary)
        {
            var isPairValueNull = v is null;
            if (
                isPairValueNull && isValueNull ||
                !isPairValueNull && v!.Equals(value)
            )
            {
                return k;
            }
        }

        throw new KeyNotFoundException($"Cannot find {value} in the dictionary values");
    }

    public List<K> Keys => new(_dictionary.Keys);
}
