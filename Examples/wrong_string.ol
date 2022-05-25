/*
  Example with strings and comments
  (both multiline and single-line)
*/
class Main is
  this() is
    // Create a string and print some symbols
    var hello = "hello\" world
    IO().WriteLine(hello.At(0))
    IO().WriteLine(hello.At(5))
  end
end