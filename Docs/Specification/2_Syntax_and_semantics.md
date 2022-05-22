# Syntactic and Semantic Structure

## Source code files

The source code in the O Language should be written in text files with `.ol` extension, yet it is not mandatory.

The file should contain all the necessary class definitions — there is no way to split a program into multiple files.

It is recommended that the source code files are named in the Snake case.

## Program structure

The program consists of one or more class declarations on the top level.

The order of class declarations within a source file does not matter.

## Class definition

Class definition should start with a `class` keyword, followed by a class name.

Optionally, one may include:

- A list of generic types used in this class, listing the names in the angle brackets `<` `>`,
- Location of this class in the inheritance tree, using `extends` keyword, followed by the base class name.

Then, the class body should be defined with `is`/`end` block.

Here is an example of a class `Name` that uses one generic type `T` and inherits from  `Parent`:

```ts
class Name<T> extends Parent is
    // Empty body.
end
```

A class can have zero or more members: fields, methods, and constructors.

The order of member declarations within a class does not matter

### Fields

Fields are used to store the state of an object.

By default, all fields are read-only outside a class, and can be modified only in the bodies of methods and constructors of the class.

#### Field definition

Fields inside a class are defined using `var` keyword, and are given an initial value after `=`.

Optionally, one might explicitly state the field type using a colon `:` **after** a variable name and **before** the initial value:

```ts
class Example is
    var a = 5                         // the type is inferred from the value
    var b: Integer = "5".ToInteger()  // the type is explicitly stated
end
```

### Methods

Methods are defined using `method` keyword, followed with the method name and the method [parameters](#method-parameters) afterwards.

If the method has a return type, it must be stated using `:` after the argument list.

Body of the method is defined using `is`/`end` block. If the method has a return type, it is required to have a [`return` statement](#return) with the value of this type in the body on all execution paths.

The body of a method can contain [statements](#statements):

- [Variable definition](#variable-definition)
- [Assignment](#assignment)
- [Conditional block](#conditional-statements)
- [Loop](#loops)
- [Return](#return)

#### Method parameters

Each parameter must be defined as a name, followed by a colon `:`, and a type for this parameter.

These definitions should be listed using comma `,`, and whole this list should be enclosed in parentheses `(` `)` and put after the method name.

For example: `method SomeMethod(param1: Integer, param2: String)`

**Note:** If there is no parameters, the parentheses are still required.

#### Example

Here is an example of a class with 1 [constructor](#constructors) and 2 methods:

```ts
class Counter is
    var a = 0
    this(value: Integer) is        // constructor with 1 parameter
        a = value
    end
    
    method Increment() is          // method without a return type
        a = a.Plus(1)
    end
    
    method IsOdd(): Boolean is     // method with a return value of type Boolean
        return a.Mod(2).Equals(1)
    end
end
```

### Constructors

A **class constructor** a special [method](#methods) used to initialize the class. It does not have a return type, and is executed only when the object of the class is being created.

#### Constructor definition

A class can have zero or more constructors: with parameters or without them, i.e. *empty constructors*.

Constructors are defined using `this` keyword, followed by a set of parameters in parentheses `(` `)`, and the constructor body using `is`/`end` block. Note that *empty constructors* still need to have parentheses after `this` keyword.

Here is an example of a class with an *empty constructor* and a constructor with a single `Integer` parameter:

```ts
class NumberWrapper is
    var number = 0
    this() is
        this.number = -1
    end

    this(number: Integer) is
        this.number = number
    end
end
```

If there is no *empty constructor* defined, a no-op empty constructor is automatically generated at compilation time:

```ts
this() is end // Auto-generated constructor
```

In the example with `NumberWrapper`, a no-op constructor would simply leave the `number` field storing `0` in it.

## Statements

### Variable definition

Variables are defined the same way as [fields](#fields), but it is done in method body.

Variables cannot be referenced outside **the block** they are defined in.

```ts
method IsGreater(a: Integer, b: Integer): Boolean is
    if a.Greater(b) then
        // This variable is accessible only within "then" block
        var greater = true
    else
        var less = true
    end
    return greater.ToString()  // Error: variable "greater" is not defined
end
```

### Assignment

Values are assigned to variables and fields using the equals sign `=`.

Value **must be of the same type** the variable or field is defined with.

If a [method call](#method-calls) is used as a value, the result of this call will be used.

If a [field reference](#field-reference) or a variable name is passed as value, their value will be copied.

#### Variable assignment

To assign a value to an existing variable, write its name and the value, separated with the equals sign `=`.

```ts
var a = 6
a = a.Plus(8)  // result of a.Plus(8) is stored back into a
```

#### Field assignment

Fields are assigned in the same way as variables, but a keyword `this` is used to denote that a class field should be used instead of local variable.

```ts
class NumberWrapper is
    var number = 0

    this(number: Integer) is
        this.number = number  // Field assignment
    end
end
```

### Conditional statements

Conditionals are defined using the following syntax: `if (condition) then (statements) [ else (statements) ] end`.

`condition` must be a value of `Boolean` type, and `statements` is a set of statements that are valid within the method body.

`then` block is **mandatory** and defines a body that will be executed if the `condition` evaluates to `true`. This block is closed with keywords `end` or `else` on the same nesting level.

`else` block is optional and defines the body to execute is `condition` evaluates to `false`. This block is closed by an `end` on the same nesting level.

Here is an example of a class using conditionals in its methods:

```ts
class Positive is
    method IsPositive(a: Integer): Boolean is
        if a.Greater(0) then
            // "then" block only, no "else"
            return true
        end
        return false
    end

    method PrintIsPositive(a: Integer) is
        if this.IsPositive(a) then
            // "then" block
            IO().Write(a.ToString().Concatenate(" is positive!"))
        else
            // "else" block
            IO().Write(a.ToString().Concatenate(" is not positive."))
        end
    end
end
```

### Loops

The only loops in The O Language are `while`-loops, which have their body executed repeatedly unless the condition evaluates to `false`.

The syntax for these is `while (condition) loop (statements) end`, where `statements` is the body of the loop, which is going to be repeatedly executed.

```ts
var numbers = ""
var i = 0
while i.Less(5) loop
    numbers = numbers.Concatenate(i.ToString()).Concatenate(" ")
    i = i.Plus(1)  // This will stop the loop after 5 iterations
end
```

### Return

The `return` keyword allows to stop executing the current method and return the flow to the place method is called.

If the current method has a return type, `return` statement with a value after it **must** finish the method execution.

Otherwise, when there is no return type, `return` must not have a value after it.

```ts
class Positive is
    method IsPositive(a: Integer): Boolean is  // has a return type Boolean
        if a.Greater(0) then
            return true                        // return with a Boolean value
        end
        return false                           // another return with a Boolean value
    end

    method PrintIsPositive(a: Integer) is      // no return type
        if this.IsPositive(a) then
            IO().Write(a.ToString().Concatenate(" is positive!"))

            // Will stop the method execution,
            // so the next IO().Write() is not reached
            return
        end
        IO().Write(a.ToString().Concatenate(" is not positive."))

        // return is not required, since there is no return type
    end
end
```

## Expressions

### Object creation

To create a new object (call a constructor), use the class name, followed by a set of arguments to pass to a constructor of the object. The constructor that matches the argument types will be chosen and invoked:

```ts
class NumberWrapper is
    var number = 0

    this(number: Integer) is
        this.number = number
    end

    method isOdd(): Boolean is
        return this.number.Mod(2).Equals(1)
    end
end

class Main is
    this() is
        // Create an object of type NumberWrapper, passing 10 to the constructor
        var wrapper = NumberWrapper(42)  

        IO().WriteLine(
            "The wrapper object stores ".Concatenate(
                wrapper.number.ToString()
            )
        )
    end
end
```

### Method calls
  
Methods of objects are called using the name of the object, followed by a dot `.`, and the method name.

Arguments passed in parentheses `(` `)` after the method name. If there are no arguments to pass, the parentheses are still required.

```ts
var n = NumberWrapper(10)  // Create a new NumberWrapper, passing 10 as argument
var odd = n.IsOdd()        // Call IsOdd() method of the NumberWrapper object
```

### Field reference
  
Object fields are referenced in the similar way as methods: name of the object is followed by a dot `.`, and the field name.

```ts
// Create a new NumberWrapper, passing 13 as argument
var n = NumberWrapper(13)

// Get the "number" field of the NumberWrapper object, 
// and call UnaryMinus() on its value.
var negative = n.number.UnaryMinus()
```
