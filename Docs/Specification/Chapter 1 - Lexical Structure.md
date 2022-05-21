# Lexical Structure

## Character set

The O Language on the syntax level supports the whole Unicode range, to the extent of `.NET 6` support of Unicode.

However, characters of *reserved language words* lie in the range of printable ASCII characters.

## Input Elements

The input text of the program is split on tokens.

For now, the possible tokens are:

- [Whitespaces](#whitespaces)
- [Comment delimiters](#comments)
- [Identifiers](#identifiers)
- [Keywords](#keywords)
- [Literals](#literals)
  - [Boolean literals](#boolean)
  - [Integer literals](#integer)
  - [Real literals](#real-floating-point)
  - [String literals](#string-literals)
- [Separators](#separators)
- [Delimiters](#delimiters)

## Whitespaces

The recommended maximum line length is `120` symbols.

The recommended tab size is `4` symbols. However, it is recommended to use **spaces instead of tabs**.

Whitespace characters are matched using [Char.IsWhiteSpace](https://docs.microsoft.com/en-us/dotnet/api/system.char.iswhitespace?view=net-6.0)
method, so whitespaces are not restricted only to usual spaces and line feed characters.

## Comments

There are two types of comments: **single-line** and **multi-line comments**.

The comments are treated as a **whitespace** (in other words, they are fully **discarded**).

### Single-line comments

**Single-line comments** start with the **double slash**: `//`.
Everything to the right of the double slash is considered a comment, **including the double slash**.

```c
// This is a comment.
class Main is    // Everything before slashes remains, the rest is discarded.
    // This is the empty constructor:
    this is
    end

    // The declaration will be ignored because it is commented out:
    // var number = 0
end
```

Code style recommendations:

- Every sentence in the comment should start with a capital letter.
- Every single line comment should end with a punctuation mark.
- When the line is too long, it is recommended to break it — in this case, the line of comment may not end with a punctuation mark.
- There should be a space after the `//` (see the example).

### Multi-line comments

Multi-line comments are wrapped between the header `/*` and the footer `*/`. Everything between the header and the footer is considered a whitespace (i.e. fully discarded).

```c
/*
 * A long description of a class can be provided here.
 * Notice the `*` on the left of this line - it is not necessary, but recommended.
 * On the next line, stars continue to line up too.
 */
class Magic is
    method getMagicNumber() is
        return 42
    end

    /* This method (and this line) is commented out:
    method getNonMagicNumber() is
        return 7
    end
    */
end
```

Multi-line comments **do not support nesting**:

```c
/* This is commented out.
  /* This might look like a nested comment,
     But it's actual header is on the first line.
  */
  * The footer on the previous line has closed the comment already, so there is a syntax error.
  * The footer on the next line will not save from it.
  */
```

Multi-line comments can be used as single line comments too:

```c
/* This is a single line comment. */
class Main is
    var value : /* Bad practice but works. */ 5
end
```

Code style recommendations:

- Sentence rules from single-line comments apply to multi-line comments.
- There should be a space after `/*` and before `*/`.
- Use star symbols to align each line of the comment. Align stars vertically. Note the spaces after each star (refer to the first example).

## Identifiers

Identifier is an unlimited-length character sequence that represents a name for a variable, class, method, or parameter.

It can consist of Unicode **letters** and **digits**, as well as **underscores** (`_`).

An identifier can only **start with a letter or underscore**, and it **cannot start with a digit**.

An identifier **cannot** have the same name as a **keyword** or a **built-in literal**.

Identifiers are **case-sensitive**.

Examples:

```c
// Valid identifiers:
first_name
変数
имя
ClassName1_magic
_good_Enough_name_for_parameter_1
_1_PLUS_1_EQUALS_10
_   // Often used to discard a value.


// Invalid identifiers:
1st_name
separate identifiers // Most likely a syntax error because there are two of them.


// The following identifiers are all different,
// because identifiers are case-sensitive:
name
Name
nAmE
NAME
```

## Keywords

Keyword is a reserved identifier that has a special meaning and function in the language.

The following keywords are **reserved**, and **cannot be used as identifiers**:

```c
class extends this method is base
if then else
while loop
end
var
return
```

### Built-in literals

Note that `true` and `false` might appear to be keywords, but they are technically **Boolean literals**:

```c
true false
```

## Literals

Literal is a source code representation of a value of a primitive type.

Each literal belongs to one and only one of the built-in types.

### Integer

Integer literal is a sequence of digits that represents integer value.

Since the `Integer` type is a standard **.NET 32-bit signed integer**, the values of integer literal should comply with the following rule:
> $-2^{32} \leq n \leq 2^{32}$, where $n$ is the `Integer` value

### Real (floating point)

Real literal represents floating point values.

It is represented with a sequence of two or more digits separated by the dot `.`, for example: `0.0` or `59009.1707`.

For the clarity sake, C-style shortcuts such as `.5` and `5.` are not permitted.

Since the `Real` type is a standard **.NET 32-bit single precision floating-point number**, positive values it can represent should comply with the following rule:

> $1.175494351 \cdot 10^{-38} \leq p \leq 3.402823466 \cdot 10^{38}$, where $p$ is the `Real` value

### Boolean

A `Boolean` type has only two values: `true` and `false`. Both of them consist of **Latin ASCII letters**.

These two literals are **reserved**, and **cannot be used as identifiers**.

However, the following identifiers are valid (although it is **NOT recommended** to name variables this way):

```typescript
True
true_
fALSE
_false
```

### String literals

A string literal consists of zero or more characters enclosed in double quotes:

```typescript
""   // Empty string.
"The quick brown fox jumps over the lazy dog!"
"The cost is 5$, but with a 50% discount it's 2.5$."
```

Note that line feed character in the string is completely valid:

```typescript
"This is an example of a string that
spans across multiple lines"
```

String literals are always of a `String` type.

## Separators

### Statement separators

There is no separators for statements.

### Block separators

#### Classes and methods

Bodies of methods and classes are separated by keywords `is` and `end`:

```c
class Example is
    this() is 
        IO().Write("I'm alive")
    end
end
```

#### Loops

Body of `while` loops is separated by keywords `loop` and `end`:

```c
while i.Less(5) loop
    IO().Write(i.ToString())
    i = i.Minus(1)
end

#### Conditionals

Body of `if` conditional is separated by keywords `then` and `end`:

```c
if a.Greater(5) then
    a = 5
end
```

If the conditional includes an `else` body, a triplet `then`/`else`/`end` is used:

```c
if a.Greater(0) then
    message = "Number is positive"
else
    message = "Number is negative or 0"
end
```

## Delimiters

### Dot

Dot `.` is used to access methods and fields of the object in the form `<Object>.<Member>`:

```typescript
var a: Integer = 5
var b = a.Plus(1)  // call method Plus of the object a, which is of type Integer
```

### Equals

Equals sign `=` is used to give a variable or field initial value,
as well as to assign the new value to existing variable or field:

```typescript
var a = 1  // set initial value of a to 1
a = 42     // store 42 in variable a
```

### Colon

Colon `:` is used to

- denote the type of a function parameter,
- denote the type of a variable,
- denote the return type of a method,
- split a key-value pair in [dictionary](Chapter%202%20-%20Types.md#Dict) definition.

```typescript
class Example is
  var list: List<Integer> = []  // Type hint for a variable
  var dict = {"amogus": 25565}  // Key-value pair in the dictionary

  method Lookup(key: String)    // Function parameter type definition
    : Integer is                // Function return type definition
    return dict.Get(key)
  end
end
```

### Parentheses

Parentheses `(` `)` and commas `,` are used to define methods parameters and to pass parameters on method calls.

If a method **is called** without arguments, parentheses are still necessary:

```typescript
Integer(1).Plus(1)    // returns 2
Integer(1).ToString() // returns "1"
Integer(1).ToString   // is just a method reference — method is not called
```

### Angle brackets

Angle brackets `<` `>` are used to denote generic types:

```typescript
class SomeGeneric<K> is
  var listOfK: List<K> = []
  var listOfIntegers: List<Integer> = []
end
```

### Square brackets

Square brackets `[` `]` are used for defining [lists](Chapter%202%20-%20Types.md#List):

```typescript
var emptyList = []
var list = [13, 27, 39, 42]
```

### Curly brackets

Curly brackets `{` `}` are used for defining [dictionaries](Chapter%202%20-%20Types.md#Dict):

```typescript
var emptyDict = {}
var dict = {"1": 20, "sus": 42}
```

## Operators

By the language design, all regular symbolic unary and binary operators, often found in other languages have been replaced with methods with descriptive names.

So, instead of the following:

```typescript
a = a * 7 + 9
```

One should write:

```typescript
a = a.Mult(7).Plus(9)
```
