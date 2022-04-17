# Project O: an Object-oriented programming language

This document assumes that the reader is familiar with the basic concepts of programming and Object-oriented paradigm.

For the language syntax, please refer to formal grammar or code examples.

## Program

The program is a set of class declarations, i.e., in the global scope, there can only be class declarations.

The entry point of the program is any constructor of any class. A user must specify the entry point of a program (the name of the class) as the first command-line argument. Parameters for the constructor should also be supplied.

The program will create an unnamed instance of this class with this constructor and transfer control to it. The program finishes executing when the specified constructor returns (finishes).

## Object-oriented programming rules

### About types

The only concept of the type system in Project O is class ("a class is a type").
Therefore, to define a new type, a programmer has to declare a new class.

Simply put, every type is a class (including base types). Thus, every object is an instance of some class.
Every type is a descendant of a base class (called `Class`).

O Language is typed, so the type of every variable is uniquely determined by its initial value and type keyword.

Class is a collection of logically related resources (**fields**) and some action on them (**methods**). Fields define the current state of object, and methods define its behavior.

**Constructors** are special methods that perform initialization of all fields in class.

### Class declarations

At compile time, a compiler should know (i.e. a programmer should define):
  - Name, the type of **fields**, and their initial values.
  - Names of **methods**, possibly followed by a list of parameters (which obey the rules for fields), and a return value if it's not void.
  - Special methods - **Constructors**.

Note: class declaration can be **empty**.

### Inheritance

Inheritance mechanism is implemented with `extends` keyword.

A child class has all the fields and methods (including constructors) from the parent class. They can be freely accessed inside a child class.

Moreover, a developer can extend a child class by adding new fields and methods.

Formally speaking, inheritance relationship is transitive: `C -> A` iff `B -> A` and `C -> B` (where `->` operator defines `inherits from` relationship).

Multiple inheritances are evil and are thus strictly prohibited.

### Polymorphism

Objects of a child class can always be cast to a parent class type. However, the inverse cast (from parent to child) is allowed only when the object was created with type of a child class and previously cast to a parent class type.

Within a single class, a programmer is allowed to define **multiple methods** with **identical name and return type**, but **different input parameters**. Such a case is called **method overloading**, and these methods are treated as different entities. Note that fields cannot be overloaded.

In a child class, a programmer can define methods with **fully identical signature** to a parent class. In this case, the method of the parent will be **overridden** by the method of the child.

When an object is cast to a parent type, and then an overridden  method is called, the method of the child type will be chosen.

Methods with identical parameters but different return types are **not** allowed.

### Encapsulation

There is no explicit mechanism of `public`, `protected`, and `private` fields or methods. However, they are implicitly implemented as follows:
  - Every field is public.
  - Fields do not support write operations from outside the class. New values can be written only in methods of this class.
  - However, reading fields are allowed from anywhere.
  - All methods are public.
  - Calling methods of read-only fields are allowed. The programmer is responsible for side-effects.

### Abstraction

The language does not support Templates (C++) or Generics (C#/Java).

## Comments

Single line comments start with `//` and are followed by single line string of any length. Everything after `//` is ignored by compiler.

Multi-line comments must start with `/*` and end with `*/`. Text of any length inside of comment will be treated as whitespace.

Example:

```
/* This is a multi-line comment.
 * Following this style is recommended.
 * Also, be nice and use dots, please.
 */
class Example is
    this(v: Integer) is
        value := v  // This is a single line comment.
    end

    var value: Integer
end
```

Nested comments are not allowed:

```
/* This part is okay.
/* Comment start literal is ignored on this line.
 * On this line, comment ends. */
 * And on this line there is a syntax error. */
```

## Language constructs

The language defines the minimum set of operators necessary for real programming: assignment, loop, conditional operator, and return operator from a method.

### Expressions

Expressions in Project O are intentionally simplified, and can be built out of two things:
  - Access to class member.
  - Method call.

Access to a class member is done via dot notation:
  - Fields: `class_instance.field`
  - Methods: `class_instance.method(param_1, param_2)`

As visible from the previous example, methods calls are done via accessing methods as members and invoking them by putting a comma-separated list of parameters in parentheses.

### Assignment

Assignment operator `:=` can be used to write a value to a variable.

Left hand side of the assignment operator must be a class field. Right hand side of the assignment operator must be any expression.

### Conditional operators

The language defines operators for conditional execution as follows:

```
if <expression> then
    // Body 1.
else
    // Body 2.
end
```

Notes:
  - `expression` after `if` statement can only be of a boolean type.
  - `Body 1` will be executed if the `<expression>` is `true`, otherwise the control will be transferred to `Body 2`.
  - `end` closes the `if` block to avoid ambiguities for nested conditional execution blocks, and marks the textual completion of the statement.

### Loops

The language has only `while` loop which continues execution as long as the `expression` is `true`:

```
while <expression> loop
    // Body
end
```

Notes:
  - The type of expression can only be boolean.
  - The expression is evaluated each time before the body of the loop.
  - The loop can execute zero or more times.

### Return statement

The `return` statement marks the end of execution for a method. After `return` is executed, the control will be transferred to a caller method.

If the callee can return a value of some time, it must be specified after `return` statement. In this case, the method call will be "replaced" in caller with the return value of a callee.

```
return [expression]
```

## Strings

String literals are denoted by wrapping any text with double quote symbol `"` as follows:

```
var text : "This is a string literal"
```

Strings are mutable because technically they are arrays of Integers, which allows for built-in Unicode support.

## Standard library

The tandard library includes a number of classes that are present in any program by default (so there is no need to explicitly import them).

### List of types for storage and algorithms

```
class Class is ... end
class AnyValue extends Class is ... end
class Integer extends AnyValue is ... end
class Real extends AnyValue is ... end
class Boolean extends AnyValue is ... end
class AnyRef extends Class is ... end
class Array extends AnyRef is ... end
class List extends AnyRef is ... end
```

### Input/Output mechanisms.

I/O is implemented via `class IO is ... end`
