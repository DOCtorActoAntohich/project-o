# Introduction

`The O Language`, or `The Project O` is a general-purpose class-based Object-Oriented language.
The initial language design was influenced by other languages such as Pascal and C#.
The target platform for the O Language programs is `.NET 6` (and all systems that can run it), which defines `C#10` as the initial implementation language.

The key design idea of The Project O is to avoid **runtime errors** caused by **implicit type casts**. This implies that The O Language is **statically and strongly typed**.
Another design idea is to **remove all operators** and clearly define the behavior of objects through methods.

## Definitions

This section contains the definitions used across the language specification.

### Typing

- **Statically typed language**: a language in which the type of every variable is known at compilation time, and cannot change after that.
- **Dynamically typed language**: a language in which the type of a variable may change at the runtime.
- **Strongly typed language**: a language that does not allow implicit type conversions, so the values of one type cannot be interpreted as the values of another type without explicit programmer's interference.
- **Weakly typed language**: a language that can implicitly convert types by interpreting values of one type as values of another type.

- **Function (method) parameters**: names listed in the function definitions, together with their types.
- **Function (method) arguments**: actual values that are passed in the function call.
### Naming conventions

- **Snake case**:
  - Lower case letters only.
  - Words separated by underscores.
  - All abbreviations are written in lower case too.
  - Example: `advanced_http_server`
- **Pascal case**:
  - Each word starts with an upper case letter, but all other letters are in lower case.
  - No spaces between words, no underscores.
  - All abbreviations are written in upper case.
  - Example: `AdvancedHTTPServer`
- **Camel case**:
  - Same as Pascal case, but the first word or abbreviation in the name is written in lowercase.
  - Example: `advancedHTTPServer` or `httpClientQueue`
- **Constant case**, or **Screaming case**:
  - Same as Snake case, but all the letters are in the upper case.
  - Example: `MAX_PLAYER_HEALTH`

### Other

- **Module** - a building unit of a program, usually represents a single file.

## Memory model

## Program compilation and execution
