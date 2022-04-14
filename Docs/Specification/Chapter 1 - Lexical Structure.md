# Lexical Structure

## Character set

The O Language on the syntax level supports only the following ASCII characters:

```
A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
a b c d e f g h i j k l m n o p q r s t u v w x y z
0 1 2 3 4 5 6 7 8 9
( ) { } [ ] < > _ - . , : ; = " #
```

If the parser meets a symbol that is not listed above, the compilation should fail with the unexpected symbol error.

Additionally, string literals may contain the following characters within themselves:

```
! @ # $ % ^ & ? * - + = / \ | ` ~ '
```

In other words, the parser will accept the code if these characters are inside string literals. If the parser finds these symbols outside string literals, or if there are any other symbols that were not listed, the compilation will fail with unexpected symbol error.

## Input Elements

## Whitespaces

The recommended maximum line length is `120` symbols.

The recommended tab size is `4` symbols. However, it is recommended to use **spaces instead of tabs**.

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

    // The declaration will be ignored
    // because it was commented out:
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
     But it's actual header is on the first line
  */
  * The footer on the previous line has closed the comment already, so there is a syntax error.
  * The footer on the next line will not save from it.
  */
```

Multi-line comments can be used as single line comments too:

```c
/* This is a single line comment*/
class Main is
    var value : /* Bad practice but works */ 5
end
```

Code style recommendations (not mandatory):
  - Sentence rules from single-line comments apply to multi-line comments.
  - There should be a space after `/*` and before `*/`.
  - Use star symbols to align each line of the comment. Align stars vertically. Note the spaces after each star (refer to the first example).

## Identifiers

## Keywords

## Literals

### Integer

### Real (floating point)

### Boolean

### String literals

### Separators

### Operators
