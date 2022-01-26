class Main is
  this is
    var numbers : Array[Real](3)
    numbers.set(0, 5.15)
    numbers.set(1, 22.05)
    numbers.set(2, 15.22)
    Calculator[Real].sum(numbers)
  end
end

class Calculator[T] is
  method sum(numbers: Array[T]) is
    var result : T(0)
    i := 0
    while i.Less(numbers.Length) loop
      result := result.Plus(numbers.get(i))
    end
    return result
  end
end