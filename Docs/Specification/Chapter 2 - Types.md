# Types

Project O is a **statically typed** language, which means that the types of all variables must be known at compile time.

```java
class Main is
    this is
        // Variable `my_var` has type String and value "hi there".
        // It can not change its type after being declared.
        var my_var = "hi there"
        my_var = 3.1415  // <-- Compile time error: Cannot assign type Real to type String
    end
end
```

Language has several builtin types. There are:

+ `String`
+ `Integer`
+ `Real`
+ `Boolean`
+ `List[T]`
+ `Dict[K, V]`

Let us briefly consider each type: 

### String

Mutable string type. You can change it, 
concatenate with another string, convert it to real or integers 

```java
var hello = "hello"
hello = hello.Concatenate(", world")
```

### Integer

Numerical type, represents integer number

```java
var number = 31415
number = "1231".ToInteger()
```

### Real 

floating-point numbers, which are numbers with decimal points.

```java
var real_number = 3.1415
real_number = "3.1415".ToInteger()
```

### Boolean

As in most other programming languages, a `Boolean` type has two possible values: `true` and `false`.


```java
var is_kek = true
var is_lol = "false".ToBoolean()
```

### List

List is compound type, meaning it group multiple values in one type.

```java
var lst: List[Integer] = []              // empty list of integers
lst = [1, 2, 3]                          // list with 3 integers 
var last = lst[lst.Length().Minus(1)]    // access to last item
```


### Dict

As in most other programming languages, type `Dict[K, V]` 
stores a mapping of keys of type `K` to values of type `V`


```java
var dct: Dict[String, Integer] = {}     // empty dict
dct = {"42": 42}                        // dict with 1 key-value pair
dct.Set("aboba", 1)                     // set value to key
dct.Get("aboba")                        // get value from key
dct.Keys()                              // return List[K]
```
