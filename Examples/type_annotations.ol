class Main is
    this is
        var list1: List<Integer> = [] // Explicit type annotation
        list1.Append(20)
        var list2 = [1, 2, 3] // Type is inferred here
        IO().Write(
            list2.Get(2)
                .Plus(
                list1.Get(
                    list1.Length().Minus(1)
                )
            )
        )
    end
end