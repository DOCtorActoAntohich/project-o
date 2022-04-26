# Syntactic and Semantic Structure

## Modules

The source code in the O language must be written in text files with `.ol` extension.

Each module should contain only one class, although this is not mandatory.
The names of source code files should be written in Snake case, and the name should be the same as the main class name.

## Importing other modules

Importing a module is done with `#import <module_name>` command. The file format is not required. The `#import` commands must be in the beginning of the file.

To import a module from another folder, write a relative path to it. The path separator is `.`.

Importing from a parent folder is not allowed. However, it is possible to supply extra import directories as a command line argument.

```Java
#import math
#import root_folder.nested_folder.file_name

class Main is
    // ...
end
```

`#import` command **copies** the content of a specified file and **pastes** it **in place of itself**.
The mechanism of imports **tracks imported files by full path** and **copies them only once**, so there is no need for programmers to worry about **circular dependencies** and **repeated definitions**.

## Program structure

The program consists of zero or more `#import` commands, followed by class declarations on the top level.

The order of class declarations within a module does not matter.

## Class definition

A class can have zero or more members: fields, methods, and constructors.
If no members are specified, the default empty constructor is automatically generated.

The order of member declarations within a class does not matter.

Class definition should start with a `class` keyword, followed by a class name, and its relations to other types. Then, the class body should be defined with `is`/`end` block:

```Java
class Name<T> extends Parent is
    // Empty body.
end
```

## Field definition

Fields inside a class are defined using `var` keyword, and are given an initial value after `=`.

Optionally, one might explicitly state the field type using ':' **after** a variable name and **before** the initial value:

```java
class Example is
    var a = 5                         // the type is inferred from the value
    var b: Integer = "5".ToInteger()  // the type is explicitly stated
end
```

## Method definition

Methods are defined using `method` keyword, with arguments passed in parentheses after the method name. If there is no arguments, the parentheses are still required. The method may also have a return type defined using `:` after the argument list.

Body of the method is defined using `is`/`end` block. If the method has a return type, it is required to have a `return` statement with the value of this type in the body on all execution paths.

Special case is a constructor of the class, which is defined using `this` keyword and never has a return value (see the example). 

The body of a method or a constructor can contain expressions, conditional statements and loops, as well as `return` statements. 

```java
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
## Expressions

### Variable definitions 
Variables are defined the same way as fields, but it is done in method body.

Variables cannot be referenced outside the block they are defined in.

```java
method IsGreater(a: Integer, b: Integer) is
if a.Greater(b) then
    var greater = true             // This variable is accessible only within "then" block
else
    var less = true
end
var result = greater.ToString()  // Error: variable "greater" is not defined
end
```
### Variable assignments 
Assignment is stated using `(name) = (value)` syntax.
```java
var a = 6
a = a.Plus(8)
```
Value **must be of the same type** the variable is defined with.

If a method call is used as a value, the result of this call will be used.

If a field reference or a variable name if passed as value, the value will be copied.

### Method calls
  
Methods are called using their name, with an arguments passed in parentheses `(` `)` after the method name.

To create a new object (call a constructor), use the class name instead of the method name:
```java
var c = Counter(10)  // Create a new Counter, passing 10 as argument
c.Increment()        // Call Increment() method of the Counter instance
```
  
## Conditional statements

Conditionals are defined using the following syntax: `if (condition) then (statements) [ else (statements) ] end`.

`condition` must be a value of `Boolean` type, and `statements` is a set of statements that are valid within the method body.

`else` block is optional and defined the actions to perform is `condition` evaluates to `false`.

```java
method IsPositive(a: Integer): Boolean is
    if a.Greater(0) then
        return true
    end
    return false
end
```

## Loops
The only loops in The O Language are `while`-loops, which have their body executed repeatedly unless the conditions evaluates to `false`.

The syntax for these is `while (condition) loop (statements) end`.

```java
var numbers = ""
var i = 0
while i.Less(5) loop
    numbers = numbers.Concatenate(i.ToString()).Concatenate(" ")
    i = i.Plus(1)  // This will stop the loop after 5 iterations
end
```
