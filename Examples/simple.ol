class Main is
  var mogus : 9
  this is
    var a : 7
    IO.WriteLine(Calculator().magic(a).ToString())
    a := Integer(5).Plus(Integer(7)).Plus(Integer(8))
    this.mogus := Integer(5).Plus(Integer(7)).Plus(Integer(8))
    var printer : IO
    printer.WriteLine(this.mogus.ToString())
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
