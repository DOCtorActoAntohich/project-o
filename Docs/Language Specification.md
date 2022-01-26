# Project O: an Object-oriented programming language

This document assumes that the reader is familiar with the basic concepts of programming and Object-oriented paradigm.

For the language syntax, please refer to formal grammar or code examples.

## Program

The program is a set of class declarations, i.e. in the global scope there can only be class declarations.

The entry point of the program is any constructor of any class. A user must specify the entry point of a program (the name of the class) as the first command-line argument. Parameters for constructor should also be supplied.

The program will create an unnamed instance of this class with this constructor and and transfer control to it. The program finishes executing when the specified constructor returns (finishes).

## Object-oriented programming rules

### About types

The only concept of type system in Project O is class ("a class is a type").
Therefore, to define a new type, a programmer has to declare an new class.

Simply put, every type is a class (including base types). Thus, every object is an instance of some class.
Every type is a descendant of a base class (called `Class`).

O Language is typed, so the type of every variable is uniquely determined by its initial value.

Class is a collection of logically related resources (**fields**) and some action on them (**methods**). Fields define the current state of object, and methods define its behavior.

**Constructors** are special methods that perform initialization of all fields in class.

### Class declarations

At compile time a compiler should know (i.e. a programmer should define):
  - Name, the type of **fields**, and their initial values.
  - Names of **methods**, possibly followed by a list of parameters (which obey the rules for fields), and a return value if it's not void.
  - Special methods - **Constructors**.

Note: class declaration can be **empty**.

### Inheritance

Inheritance mechanism is implemented with `extends` keyword.

A child class has all the fields and methods (including constructors) from the parent class. They can be freely accessed inside a child class.

Moreover, a developer can extend a child class by adding new fields and methods.

Formally speaking, inheritance relationship is transitive: `C -> A` iff `B -> A` and `C -> B` (where `->` operator defines `inherits from` relationship).

Multiple inheritance is evil, and is thus strictly prohibited.

### Polymorphism

Objects of a child class can always be casted to a parent class type. However, the inverse cast (from parent to child) is allowed only when the object was created with type of a child class and previously casted to a parent class type.

Within a single class, a programmer is allowed to define **multiple methods** with **identical name and return type**, but **different input parameters**. Such case is called **method overloading**, and these methods are treated as different entities. Note that fields cannot be overloaded.

In a child class, a programmer can define methods with **fully identical signature** to a parent class. In this case, the method of the parent will be **overriden** by the method of the child.

When an object is casted to a parent type, and then an overriden method is called, the method of the child type will be chosen.

Methods with identical parameters but different return types are **not** allowed.

### Encapsulation

There is no explicit mechanism of `public`, `protected`, and `private` fields or methods. However, they are implicitly implemented as follows:
  - Every field is public.
  - Fields do not support write operations from outside the class. New values can be written only in methods of this class.
  - However, reading fields is allowed from anywhere.
  - All methods are public.
  - Calling methods of read-only fields is allowed. The programmer is responsible for side-effects.

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

## Input/Output mechanisms.

[Not yet implemented].

## Formal Grammar

```
Program : { ClassDeclaration }

ClassDeclaration
    : class ClassName [ extends ClassName ] is
    { MemberDeclaration }
    end

ClassName : Identifier [ '[' ClassName ']' ]

MemberDeclaration
    : VariableDeclaration
    | MethodDeclaration
    | ConstructorDeclaration

VariableDeclaration
    : var Identifier ':' Expression

MethodDeclaration
    : method Identifier [ Parameters ] [ : Identifier ]
    is Body end

Parameters : ( ParameterDeclaration { , ParameterDeclaration } )

ParameterDeclaration
    : Identifier : ClassName

Body : { VariableDeclaration | Statement }

ConstructorDeclaration
    : this [ Parameters ] is Body end

Statement : Assignment
    | WhileLoop
    | IfStatement
    | ReturnStatement

Assignment : Identifier ':=' Expression

WhileLoop : while Expression loop Body end

IfStatement : if Expression then Body [ else Body ] end

ReturnStatement
    : return [ Expression ]

Expression : Primary { '.' Identifier [ Arguments ] }

Arguments : '(' Expression { ',' Expression } ')'

Primary : IntegerLiteral
    | RealLiteral
    | BooleanLiteral
    | this
    | ClassName
```