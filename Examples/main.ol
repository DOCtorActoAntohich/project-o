class Main is
  this is
    IO.WriteLine("Please, input 2 integers as parameters")
  end

  this(a: Integer, b: Integer) is
    IO.WriteLine("Sum of numbers is ".Concatenate(a.Plus(b).ToString()))
  end
end