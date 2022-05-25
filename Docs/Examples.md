# Examples of O language programs

**Note:** unless example is a set of methods or classes, it implies that these statements are put inside a main body:

```ts
class Main is
    this is
        // example body
    end
end
```

## Valid examples

1. Lists
```ts
var fibs = []
var a = 1
var b = 1
while a.Less(10000) loop
    var t = a
    a = a.Plus(b)
    b = t
    fibs.Append(b)
end

IO().WriteLine(fibs.Get(3).ToString())
```

    IO().WriteLine(fibs.Sum().ToString())
    ```

2. Counting letters in the sentence using `Dict`

    ```ts
    var string = "The quick brown fox jumps over the lazy dog"
    var count_dct = {}
    var i = 0

    // construct count dict
    while i.Less(string.Length()) loop
        var key = string.At(i)
        if count_dct.Exists(key).Not() then
            count_dct.Set(key, 0)
        end
        count_dct.Set(key, count_dct.Get(key).Plus(1))
    end

    // print dict
    IO().Write("{")
    var keys = dict.Keys()
    var i = 0
    while i.Less(keys.Length()) loop
        var key = keys.Get(i)
        IO().Write(key)
        IO().Write(": ")
        IO().Write(dict.Get(key).ToString())
        IO().Write(", ")
        i = i.Plus(1)
    end
    IO().Write("}")
    ```

## Invalid examples (compile time errors)

```ts
var lst = [] // <-- error: cannot infer type of `lst`, provide type hint
var a = lst.Get(0)
```


```ts
var fibs = []
fibs.Append(1)
fibs.Append("2") // <- error: List<Integer>.Append 
                 // expects first agrument of type Integer
```

```ts
var fibs: List<Integer> = []
fibs.Append("2") // <- error: List<Integer>.Append 
                 // expects first agrument of type Integer
```

```ts
var lst = [1, False] // <-- error: cannot construct a list from elements of different types
```

```ts
method TakesInts(lst: List<Integer>): Boolean is
    return lst.Get(0) // <-- error: TakesInt expected 
                      // to return type Boolean, found Integer
end
```

```ts
var colors = {
    "red": 0, 
    "blue": 1,
    "green": 2,
}
if colors.Exists("haskell").Not() then
    colors.Set("haskell", "bad") // <-- error: Dict<String, Integer>.Set 
                                 // expected type Integer as second argument, found String
end
```

```ts
var a = 0
if a then // <-- error: If expected type Boolean, found Integer
    a = a.Plus(1)
else
```

```ts
var a = 0
if a.Equals(0) then
    a = [1] // <-- error: cannot assign type List<Integer> to type Integer 
end
```

```ts
class Main is
    method number(): Integer is
        return 42
    end
    this is
        IO().Write(this.number()) // error: IO().Write expected String as first argument, found Integer
    end
```

```ts
while 1 loop // error: While expected type Boolean, found Integer
end
```

```ts
class Tuple<A, B> is
    var first: A
    var second: B
    this (first: A, second: B) is
        this.first = first
        this.second = second
    end
end

class PairOfSame<A>: Tuple<A, A> is
end

class Main is
    this is
        var t = Tuple(False, "cat")
        if t then // <-- error: If expected type Boolean, found Tuple<Boolean, String>
            IO().Write(t.second)
        end

        vat t2 = PairOfSame(False, "cat") // <-- error: PairOfSame constructor 
                                          // takes second argument of type Boolean, found String
    end
end
```

```ts
class Operation is
end

class War is
end

class Main is
    method Say(word: Operation): Void is
    end

    this is
        var word = War()
        this.Say(word) // <-- error: Say expected first argument of
                       // type Operation, found War
    end
end
```

```ts
class Storage<T> is
    var data: Dict<String, T>
    this is
        this.data = {}
    end
end

class Main is
    this is
        var s = Storage<List<String>>()
        s.data.Set("haskell", [1,2,3]) // error: cannot cast List<Integer> to List<String>
    end
end
```
