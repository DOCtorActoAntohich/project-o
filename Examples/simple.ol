class Main is
  this is
    var a : 7
    IO.WriteLine(Calculator().magic(a).ToString())
  end
end

class Calculator is
  this is end
  method magic(num: Integer) : Integer is
    if num.Less(5) then
      var result : num.Mult(12).Minus(num.Mult(2))
      return result.Minus(1)
    end
    return num.Plus(num)
  end
end
