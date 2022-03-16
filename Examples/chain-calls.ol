class Main is
  this is IO.Write("give me one integer pls") end 
  this(a: Integer) is
    IO.WriteLine(Calculator().magic(a).ToString())
  end
end

class Calculator is
  method magic(num: Integer) : Integer is
    if num.Less(5).Not then
      var result : num.Mult(12).Minus(num.Mult(6))
      return result
    end
    return num.Plus(num)
  end
end
