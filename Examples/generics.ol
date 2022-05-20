class Main is
    this is
        var list: List<Dictionary<String, Pair<Integer, String>>> = []
        list.Append({})
        var pair = Pair(42, "Amogus")

        list.Get(0).Set("sus", pair)
        IO().WriteLine(list.Get(0).Search(pair))
    end
end

class Pair<K, V> is
    var key: K = null
    var value: V = null

    this(key: K, value: V) is
        this.key = key
        this.value = value
    end

    method ToString(): String is
        return "Pair("
                 .Concatenate(this.key.ToString())
                 .Concatenate(", ")
                 .Concatenate(this.value.ToString())
                 .Concatenate(")")
    end
end