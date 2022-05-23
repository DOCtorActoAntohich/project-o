# Object-Oriented Programming

As the O Language makes use of classes and their instances — *objects*, the language also has some features of the Object-Oriented Programming (OOP) paradigm.

## Inheritance

Each user-defined class always inherits from some *base* class. This is also true for all the [built-in classes](3_Types.md#built-in-types), except the [special ones](3_Types.md#special-types).

### Defining inheritance

By default, any class inherits from [`Class`](3_Types.md#class). Any other base class can be specified using `extends` keyword, as described in the syntax chapter (see [Class definition](2_Syntax_and_semantics.md#class-definition)).

### Interitance rules

Inheriting from a class means that all the methods and fields of the base (parent) class are available in the inheriting (child) class.

However, the fields and methods can actually be overriden in the child class, if a method or field with the same name as in the parent were created.

### Initializing the base class

When an instance of a class is created, all the base classes up to `Class` are also initialized. If not specified otherwise, an empty constructor of the base class is called for initializing the parent class.

This behaviour can be changed in the constructor of the child class: it is necessary to use `base` keyword, followed by a list of [arguments](2_Syntax_and_semantics.md#method-calls) to the base class, in parentheses `(` `)`. This can also be useful to postpone initialization of the base class — by default, the base class constructor is called first.

### Inheritance example

## Abstraction

The O language does not define any kinds of abstract classes and interfaces, but one can create a class with empty method bodies, and use it as an abstract class.

However, inheriting such a class would not require overriding and implementing the methods of the base "abstract" class.

## Encapsulation

Encapsulation in O is expressed in the way that all the fields of object canot we written to outside of the methods of this object, i.e. they are *read-only* outside of the class.

At the same time, all the fields can be read outside of the class they are defined in, and all the methods of an object can be called outside of that object (that is, they are *public*).

For the time being, there is no way to change the default behaviour.

## Polymorphism

The O language freely supports upcasting, so using base classes to refer to objects of the inherited classes is fully operational.

However, downcasting cannot be performed.

### Polymorphism examples

## Static members

For the time being, there is no static classes and members. One need to create an instance of the class, even if the required methods do not access the instance fields.

For example, all the methods of built-in [`IO`](3_Types.md#io) and [`Time`](3_Types.md#time) could be made static, but there is no support for this feature yet.