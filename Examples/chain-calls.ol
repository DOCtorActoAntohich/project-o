class Main is
  var mogus : 9
  this is
    var a : 7
    a := Integer(5).Plus(Integer(7)).Plus(Integer(8))
    this.mogus := Integer(5).Plus(Integer(7)).Plus(Integer(8))
    var c : Calculator
    IO.WriteLine(this.mogus.ToString()) 
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
