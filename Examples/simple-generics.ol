class Main is
    this() is

    end
end

class Pair<K, V> is
    var key: K
    var value: V

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