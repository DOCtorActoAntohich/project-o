/*/* Comment /**/ /**/
/*
  Example with strings and comments
  (both multiline and single-line)
*/
class Main is
  this is
    // Create a string and take some symbols
    var hello : "hello\" world"
    hello.get(0)
    hello.get(5)
  end
end

class Integer is
end

class Real is
end

class String is
  this(num: Integer) is
  end
  this(num: Real) is
  end
  method get(position: Integer) : String is
  end
end
