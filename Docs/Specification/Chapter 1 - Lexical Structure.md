# Lexical Structure

## Character set

The O Language on the syntax level supports the whole Unicode range, to the extent of .NET Core 6.0 support of Unicode.

However, characters of reserved language words lie in the range of printable ASCII characters.

## Input Elements

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
    // var number : 0
end
```

Code style recommendations (not mandatory):
  - Every sentence in the comment should start with a capital letter.
  - Every single line comment should end with `.` or `:`
  - When the line is too long, it is recommended to break it - in this case, the line of comment may not end with `.` or `:`.
  - There should be a space after the `//` (see the example).

### Multi-line comments

Multi-line comments are wrapped between the header `/*` and the footer `*/`. Everything between the header and the footer is considered a whitespace (i.e. fully discared).

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

**Multi-line comments do not support nesting**:

```c
/* This is commented out
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

Code style recommendations (not mandatory):
  - Sentence rules from single-line comments apply to multi-line comments.
  - There should be a space after `/*` and before `*/`.
  - Use star symbols to align each line of the comment. Align stars vertically. Note the spaces after each star (refer to the first example).

## Identifiers

Identifier is an unlimited-length character sequence that represents a name for a variable, class, method, or parameter.

It can consist of **Latin ASCII letters** (uppercase `A-Z` and lowercase `a-z`), **digits** (`0-9`), or **underscores** (`_`).

An identifier can only **start with a letter or underscore**, and it **cannot start with a digit**.

An identifier **cannot** have the same name as a **keyword** or a **built-in literal**.

Identifiers are **case-sensitive**.

Examples:

```c
// Valid identifiers:
first_name
ClassName1_magic
_good_Enough_name_for_parameter_1
_1_PLUS_1_EQUALS_10
_   // Often used to discard a value.


// Invalid identifiers:
1st_name
separate identifiers // Most likely a syntax error because there are two of them.


// The following identifiers are all different,
// Because identifiers are case-sensitive:
name
Name
nAmE
NAME
```

## Keywords

Keyword is a reserved identifier that has a special meaning and function in the language.

The following keywords are **reserved**, and **cannot be used as identifiers**:

```
class extends this method is base
if then else
while loop
end
var
return
```

Note that `true` and `false` might appear to be keywords, but they are technically boolean literals

```
true false
```

## Literals

Literal is a source code representation of a value of a primitive type.

Each literal belongs to one and only one of the built-in types.

### Integer

Integer literal is a sequence of digits that represents integer value.

Since the `Integer` type is a standard **.NET 32-bit signed integer**, the values of integer literal should comply with the following rule:

![signed_integer_limit](https://user-images.githubusercontent.com/49134679/163717203-b5308489-ae24-4a85-99f7-7b9a66c4d6e7.png)

### Real (floating point)

Real literal represents floating point values.

It is represented with a sequence of two or more digits separated by the dot `.`, for example: `0.0` or `59009.1707`.

For the clarity sake, C-style shortcuts such as `.5` and `5.` are unallowed.

Since the `Real` type is a standard **.NET 32-bit single precision floating-point number**, positive values it can represent should comply with the following rule:

![positive_real_limit](https://user-images.githubusercontent.com/49134679/163717837-9164826e-7dad-4595-88ef-c1139b606ceb.png)

### Boolean

A `Boolean` type has only two values: `true` and `false`. Both of them consist of **Latin ASCII letters**.

These two literals are **reserved**, and **cannot be used as identifiers**.

However, the following identifiers are valid (although it is **NOT recommended** to name variables this way):

```c
True
true_
fALSE
_false
```

### String literals

A string literal consists of zero or more characters enclosed in double quotes:

```
""   // Empty string.
"The quick brown fox jumps over the lazy dog!"
"The cost is 5$, but with a 50% discount it's 2.5$."
```

### Separators

### Operators
