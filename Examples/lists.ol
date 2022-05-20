class Main is
  method reverse_list(lst: List<Integer>): List<Integer> is
    if lst.IsEmpty() then
      return []
    else
      var first_element = lst.Get(0)
      var reversed_rest = reverse_list(lst.Slice(1, lst.Length().Minus(1))
      reversed_rest.Append(first_element)
      return reversed_rest
    end
  end

  this is
    var lst: List<Integer> = [1, 2, 3]
    var reversed_list = reverse_list(lst)
  end
end
