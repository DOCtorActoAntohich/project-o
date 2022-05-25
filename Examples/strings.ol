/*/* Comment /**/ /**/
/*
  Example with strings and comments
  (both multiline and single-line)
*/
class Main is
  this() is
    // Create a string and take some symbols
    var hello = "hello\" world"
    IO().Write(hello)
    IO().Write(hello.At(0))
    IO().Write(hello.At(5))
  end
end
