class Main is
  method ReverseList(lst: List<Integer>): List<Integer> is
    if lst.IsEmpty() then
      return []
    else
      var first_element = lst.Get(0)
      var reversed_rest = reverse_list(lst.Slice(1, lst.Length().Minus(1))
      reversed_rest.Append(first_element)
      return reversed_rest
    end
  end

  method PrintList(lst: List<Integer>) is
    IO.Write("[")
    var i = 0
    while i.Less(lst.Length()) loop
      IO.Write(lst.Get(i).ToString())
      IO.Write(", ")
      i = i.Plus(1)
    end
    IO.Write("]")
  end

  method PrintDict(lst: Dict<String, Integer>) is
    IO.Write("{")
    var keys = dict.Keys()
    var i = 0
    while i.Less(keys.Length()) loop
      var key = keys.Get(i)
      IO.Write(key)
      IO.Write(": ")
      IO.Write(dict.Get(key).ToString())
      IO.Write(", ")
      i = i.Plus(1)
    end
    IO.Write("}")
  end

  this is
    var lst: List<Integer> = [1, 2, 3]
    PrintList(lst)
    var reversed_list = ReverseList(lst)
    PrintList(reversed_list)

    var keyBase = "sus"
    var dict: Dict<String, Integer> = {}
    var i = 0
    while i < lst.Length() loop
      dict.Set(
        keyBase.Concatenate(i.ToString()),
        lst.Get(i)
      )
      i = i.Plus(1)
    end
    PrintDict(dict)
  end
end
