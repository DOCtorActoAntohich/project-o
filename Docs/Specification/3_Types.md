# Types

Project O is a **statically typed** language, which means that the types of all variables must be known at compile time.

```ts
class Main is
    this is
        // Variable `my_var` has type String and value "hi there".
        // It can not change its type after being declared.
        var my_var = "hi there"
        my_var = 3.1415  // <-- Compile time error: Cannot assign type Real to type String
    end
end
```

## Type definition

For the sake of simplicity, the O Language does not define any way to combine types, that is, **types are defined only by classes**, and the type variety is constrained by the number of classes defined in the program.

Class definition has been described in the previous chapter, see [Class definition](2_Syntax_and_semantics.md#class-definition).

However, there are some [built-in types](#built-in-types) that give the basic functionality available for use in your source code.

## Built-in types

The O language has several built-in types:

- Primitive types:
  - [`String`](#string)
  - [`Integer`](#integer)
  - [`Real`](#real)
  - [`Boolean`](#boolean)
  - [Special primitives](#special-types)
    - [`Class`](#class)
    - [`Void`](#void)
- Compound types:
  - [`List<T>`](#list)
  - [`Dict<K, V>`](#dict)
- Utility types:
  - [`IO`](#io)
  - [`Time`](#time)

### String

```ts
var hello = "hello"
var fromInt  = String(123)  // or Integer(123).ToString()
var fromReal = String(1.5)  // or Real(1.5).ToString()
var fromBool = String(true) // or Boolean(true).ToString()
```

Immutable string type. You **cannot** change it, but you can:

- Compare it to another string:

  ```ts
  var obviouslyFalse: Boolean = hello.Compare("not hello")
  // the variable is a Boolean false, since "hello" != "not hello"
  ```

- Concatenate it with another string:

  ```ts
  hello = hello.Concatenate(", world")
  // Now the variable stores "hello, world"
  ```

- Take a symbol at a position inside the string

  ```ts
  // first character has index 0
  var w: String = hello.At(7) // .At() returns a 1-symbol String
  var e = hello.At(1)
  var l = hello.At(2)
  var well: String = w.Concatenate(e).Concatenate(l).Concatenate(l)
  ```

- Convert it to other primitives:

  ```ts
  var truth: Boolean   = "true".ToBoolean()
  var number: Integer  = "42".ToInteger()
  var realNumber: Real = "13.25".ToReal()
  ```

  If the conversion can't be done, an error at runtime happens.
  
  ```ts
  var errorCause = "not a number".ToReal() // Execution would stop here
  ```

### Integer

Numerical type, represents an integer number.

```ts
var number = 125
var fromReal = Integer(1.5)  // or Real(1.5).ToInteger()
```

Since the `Integer` type is a standard [**.NET 32-bit signed integer**](https://docs.microsoft.com/en-us/dotnet/api/system.int32?view=net-6.0), values of this type should comply with the following rule:

> $-2^{32} \leq n \leq 2^{32}$, where $n$ is the `Integer` value

#### `Integer` Methods

- Arithmetics
  
  All methods with an argument accept both `Real` and `Integer`.

  When `Real` is passed as an argument, the result is also going to be of type `Real` (unless this is a comparison, which always returns a `Boolean` value)

  ```ts
  var a: Integer = number.Plus(1)     // will store 126
  var b: Real    = number.Minus(1.0)  // will store 123.0
  var c: Integer = number.Mult(2)     // will store 250
  var d: Real    = number.Div(5.0)    // will store 25.0
  var negative = number.UnaryMinus()  // will store -125

  // Comparison
  var v = number.Less(125)         // false
  var w = number.LessEqual(125)    // true
  var x = number.Greater(125)      // false
  var y = number.GreaterEqual(125) // true
  var z = number.Equal(125.0)      // true
  ```

- Conversion to other primitives
  
  ```ts
  // false, any other value would give true
  var falsity: Boolean = Integer(0).ToBoolean()
  
  var strValue: String = number.ToString()
  var realNumber: Real = number.ToReal()
  ```

- Other

  Has 2 methods to retrieve the maximum and minimum possible values:

  ```ts
  var big = Integer().Max() // any Integer variable also has this method

  var bigButNegative = Integer().Min()
  ```

### Real

Floating-point number, that is, a number with a decimal point.

```ts
var realNumber = 3.1415
var fromInt = Real(123)
```

Since the `Real` type is a standard [**.NET 64-bit double precision floating-point number**](https://docs.microsoft.com/en-us/dotnet/api/system.double?view=net-6.0), the values it can represent should comply with the following rule:

> $-1.79769313486232 \cdot 10^{308} \leq p \leq 1.79769313486232 \cdot 10^{308}$, where $p$ is the `Real` value

Note that, however, it can hold only *15 decimal digits of precision*.

#### `Real` Methods

- Arithmetics

  Has all the arithmetic methods from `Integer`, but the calculations return `Real`, no matter if the argument is `Integer` or `Real`.

- Conversions
  
  ```ts
  // false, any other value would give true
  var falsity: Boolean = Real(0).ToBoolean()
  
  var strValue: String   = realNumber.ToString()
  var intNumber: Integer = realNumber.ToInteger() // Truncating, 3.1415 becomes 3
  ```

- Other

  Similarly to `Integer`, has 2 methods to retrieve the maximum and minimum possible values:

  ```ts
  var big = Real().Max() // any Real variable also has this method

  var bigButNegative = Real().Min()
  ```

  Also has an `Epsilon()` method, which returns the smallest possible **positive** `Real` number.

  ```ts
  var smallest = Real().Epsilon()
  ```

  *Epsilon* value is also used to determine if the `Real` is approximately equal to zero, when converting the value to `Boolean`.

### Boolean

Represents a logical value. Has two possible values: `true` and `false`.

```ts
var isKek = true
var isLol = "false".ToBoolean()
```

Supported operations:

- Logic operations
  All the logic operations return a new `Boolean` value.

  ```ts
  var and = isKek.And(isLol) // false
  var or  = isKek.Or(isLol)  // true
  var xor = isKek.Xor(isLol) // true
  var not = isKek.Not()      // false
  ```

- Conversion to `Integer` and `String`

  Conversion to `Integer` results in 1, if the value is `true`, 0 otherwise.
  
  ```ts
  var one = isKek.ToInteger()
  var zero = isLol.ToInteger()
  ```

  Conversion to `String` results in `"True"` for `true` value and `"False"` otherwise.
  
  ```ts
  var truth:   String = isKek.ToString()
  var falsity: String = isLol.ToString()
  ```

### List

`List<T>` is a compound type, meaning that it stores multiple values **of the same type** together.

```ts
var list: List<Integer> = []       // An empty list of integers
list = [1, 2, 3]                   // A list with 3 integers

var anotherList = List<Integer>()  // Also an empty list of integers
```

Note that you need to specify the type explicitly when the list is initialized with an empty list literal `[]`.

#### Methods of `List<T>`

- `Length() : Integer`

  Returns the current amount of items stored in the list.

  ```ts
  var len1: Integer = list.Length()  // 3, since list is [1, 2, 3]
  var len2 = anotherList.Length()    // 0
  ```

- `Get(index: Integer) : T`

  Get the item from the list by its index. Items are numbered from 0.

  Causes an error if the list has less items than the target index.

  ```ts
  var two: Integer = list.Get(1)           // 2

  var error = anotherList.Get(0)  // error, since anotherList is empty
  ```

- `Search(item: T) : Integer`

  Returns the index of the first occurence of the item.

  If the item is not found, returns `-1`.

  ```ts
  var s1 = list.Search(3)         // 2
  var s2 = anotherList.Search(2)  // -1
  ```

- `Set(index: Integer, item: T) : Void`

  Replaces the item at specified index with `item`.

  Causes an error if the list has less items than the target index.

  ```ts
  list.Set(0, 15) // list is now [15, 2, 3]
  ```

- `Append(item: T) : Void`

  Appends the item to the end of the list, that is, creates a new index and assigns a value associated with it.

  ```ts
  list.Append(42)            // list is now [15, 2, 3, 42]
  var newLen = list.Length() // 4
  ```

- `RemoveAt(index: Integer) : Void`

  Removes the item at specified index.

  Causes an error if the list has less items than the target index.

  ```ts
  // list is [15, 3, 42]
  var two = list.Get(1)      // 2
  list.RemoveAt(1)           // list is now [15, 3, 42]
  var three = list.Get(1)    // 3
  ```

- `Pop() : T`

  Removes the last item in the list and returns it.

  Causes an error if the list is empty.

  ```ts
  // list is [15, 3, 42]
  var sixByNine = list.Pop() // 42
  var len = list.Length()    // 2
  ```

P.S. [Why six by nine?](https://en.wikipedia.org/wiki/Phrases_from_The_Hitchhiker%27s_Guide_to_the_Galaxy#The_Answer_to_the_Ultimate_Question_of_Life,_the_Universe,_and_Everything_is_42)

### Dict

`Dict<K, V>` represents a mapping between values (*keys* of type `K` and *values* of type `V`)

```ts
var dict: Dict<String, Integer> = {}     // empty dict
dict = {"42": 42, "sus": -1}             // dict with 2 key-value pais

var anotherDict = Dict<String, Integer>()
```

Note that, similarly to lists, you need to specify the type explicitly when the dict is initialized with an empty dict literal `{}`.

#### Methods of `Dict<K, V>`

- `Length() : Integer`

  Returns the current amount of keys in the dict.

  ```ts
  var len1: Integer = dict.Length()  // 2
  var len2 = anotherDict.Length()    // 0
  ```

- `Get(key: K) : V`

  Get the item from the dict by its key.

  Causes an error if the key is not in the dict.

  ```ts
  var minusOne: Integer = dict.Get("sus") // -1
  ```

- `Search(value: V) : K`

  Returns the key of the first occurence of the value.

  If the item is not found, throws an error.

  ```ts
  var s1: String = dict.Search(42) // "42"
  ```

- `Set(key: K, value: V) : Void`

  Replaces the item under specified key with `value`.
  If the key does not exist, creates it first.

  ```ts
  dict.Set("why", 13) // dict is now {"42": 42, "sus": -1, "why": 13}
  ```

- `Remove(key: K) : Void`

  Removes key-pair value with the specified key.

  Causes an error if there is no such key in the dict.

  ```ts
  dict.Remove("42") // dict is now {"sus": -1, "why": 13}
  ```

- `Keys() : List<K>`

  Returns a `List<K>`, which contains all the keys stored in the dict.

  ```ts
  var keys = dict.Keys() // ["sus", "why"]
  ```

### IO

This *utility* type allows to interact with the user's console.

Possible interactions:

- Write out a string
  
  `Write(s: String) : Void` allows to print a string in the console.

  `WriteLine(s: String) : Void` does the same, but adds a line feed character to the output.

  `WriteLine() : Void` simply adds a line feed character to the output.

  ```ts
  // Print 10 dots and move the cursor to the next line
  var i = 0
  while i.Less(10) loop
      IO().Write(".")
      i = i.Plus(1)  
  end

  IO.WriteLine()
  ```

- Read a line from the input
  
  `ReadLine() : String` blocks the program execution and reads a line from the input, returns it and continues the execution.

  ```ts
  IO().Write("Please enter a number: ")

  // Waits for user input
  var num = IO().ReadLine("Please enter a number: ").ToInteger()
  
  IO().WriteLine("Your number mod 8 is: ".Concatenate(
      num.Mod(8).ToString()
  ))
  ```

### Time

This *utility* type allows to:

- Get the current local time:
  
  `Current() : Integer` returns the current time as a [Unix Timestamp](https://en.wikipedia.org/wiki/Unix_time).

  ```ts
  var secSinceMidnight = Time.Current().Mod(86400)
  var hours = secSinceMidnight.Div(3600)
  var minutes = secSinceMidnight.Div(60).Mod(60)
  var seconds = secSinceMidnight.Mod(60)
  var time =
      hours.ToString()
  .Concatenate(
      minutes.ToString()
  ).Concatenate(
      seconds.ToString()
  )
  
  IO().WriteLine("Current time is ".Concatenate(time))
  ```

- Pause the program execution for a fixed time:

  Calling `Sleep(seconds: Integer) : Void` would make the delay before proceeding the execution.

  ```ts
  // Print 3 dots and move the cursor to the next line,
  // but wait for 1 second before printing each dot 
  var i = 0
  while i.Less(3) loop
      Time.Sleep(1)
      IO().Write(".")
      i = i.Plus(1)  
  end

  IO.WriteLine()
  ```

### Special types

These types do not have any meaningful logic, but are important to make the O Language work.

#### `Class`

This type is a base for all other types. It defines a single method `ToString() : String`, which returns an empty string.

All the built-in classes inherit from this class, or from its descendants.

All the classes in the program in O Language inherit `Class` directly, unless this behavior is changed with the `extends` syntax.

#### `Void`

This type has no behavior at all, and represents an absence of the return value of a function.
