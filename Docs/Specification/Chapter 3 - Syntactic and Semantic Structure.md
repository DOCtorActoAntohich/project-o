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

The class can have zero or more members: fields, methods, and constructors.
If no members are specified, the default empty constructor is automatically generated.

The order of member declarations within a class does not matter.

Class definition should start with a `class` keyword, followed by a class name, and its relations to other types. Then, the class body should be defined with `is`/`end` block:

```Java
class Name<T> extends Parent is
    // Empty body.
end
```

## Field definition

## Method definition

## Expressions

## Conditional statements

## Loops
