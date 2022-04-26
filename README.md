# Project O

The O language is an Object-Oriented toy language.

A compiler for the O language is made for educational purposes.

## How to build and run programs

The target platform of the project is `.NET 6`. It is required to have it installed.

### How to compile a compiler

1. Open the `.sln` file in Visual Studio or Rider.
2. Click `Build Solution` or `Build Project`.

### How to run programs from Visual Studio / Rider

You can do either of the following:
- Create a new launch setting based on existing ones.
- Edit any program in `Examples` folder.

After that, select the desired launch configuration and press `Launch`. Your IDE should automagically build a compiler for you, and then run the program.

Also, be sure to read through the next section.

### How to run programs from the command line

*Note: currently The O language programs can only be interpreted because `.NET 6` does not allow to save binary files. We will be working on this issue soon.*

> To get familiar with the language, read more about the language documentation (see [Additional information](#additional-information)).

To run the O language program, you should save it to a file with `.ol` format.
Then, run the compiler with the following command:

```
dotnet OCompiler <module_name> <class_name> [parameters]
```

1. `module_name` is a path to your `.ol` file.
2. `class_name` is a name of class whose constructor will be an entry point of the program.
3. `parameters` is a space-separated list of parameters for the class constructor (they can only be of a primitive built-in type).

## Additional information

Refer to some other sections in the repository if needed:
  - Example programs are located in the [Examples](Examples/) folder.
  - Technical information and language specification can be found in the [Docs](Docs/) folder.
