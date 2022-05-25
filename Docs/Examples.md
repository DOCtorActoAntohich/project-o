Examples of O language programs
===

**note:** unless example is set of methods or classes, it is implies main body:

```java
class Main is
    this is
        // example body
    end
```


## Valid examples

1. Lists and `Sum()`
```java
fibs = []
a = 1
b = 1
while a.Less(10000) loop
    t = a
    a = a.Plus(b)
    b = t
    fibs.Append(b)
end

IO().WriteLine(fibs.Sum().ToString())
```

2. Dicts

```java
string = "The quick brown fox jumps over the lazy dog"
count_dct = {}
i = 0

// construct count dict
while i.Less(string.Length()) loop
    key = string.At(i)
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

```java
lst = [] // <-- error: cannot infer type of `lst`, provide type hint
a = lst.Get(0)
```


```java
fibs = []
fibs.Append(1)
fibs.Append("2") // <- error: List<Integer>.Append 
                 // expects first agrument of type Integer
```

```java
fibs: List<Integer> = []
fibs.Append("2") // <- error: List<Integer>.Append 
                 // expects first agrument of type Integer
```

```java
lst = [1, False] // <-- error: cannot construct a list from elements of different types
```

```java
method TakesInts(lst: List<Integer>): Bool is
    return lst.Get(0) // <-- error: TakesInt expected 
                      // to return type Boolean, found Integer
end
```

```java
colors = {
    "red": 0, 
    "blue": 1,
    "green": 2,
}
if colors.Exists("haskell").Not() then
    colors.Set("haskell", "bad") // <-- error: Dict<String, Integer>.Set 
                                 // expected type Integer as second argument, found String
end
```

```java
a = 0
if a then // <-- error: If expected type Boolean, found Integer
    a = a.Plus(1)
else
```

```java
a = 0
if a.Equals(0) then
    a = [1] // <-- error: cannot assign type List<Integer> to type Integer 
end
```

```java
method number(): Integer is
    return 42
end

IO().Write(this.number()) // error: IO().Write expected String as first argument, found Integer
```

```java
while 1 loop // error: While expected type Boolean, found Integer
end
```

```java
class Tuple<A, B> is
    this (first: A, second: B) is
        this.first = first
        this.second = second
    end
end

class PairOfSame<A>: Tuple<A, A> is
end

class Main is
    this is
        t = Tuple(False, "cat")
        if t then // <-- error: If expected type Boolean, found Tuple<Boolean, String>
            IO().Write(t.second)
        end

        t2 = Tuple(False, "cat") // <-- error: PairOfSame constructor 
                                 //takes second agrument of type Boolean, found String
    end
end
```

```java
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

```java
class Storage<T> is
    this is
        this.data: Dict<String, T> = {}
    end
end

class Main is
    method Sum(lst: List<Integer>): Integer is
        //
    end
    this is
        s = Storage<List<String>>()
        s.data.Set("haskel", [1,2,3])   // <---- error: cannot assign List<Integer> to List<String>
                                        //     |
        this.Sum(s.data.Get("haskell")) // <---/
    end
end
```
