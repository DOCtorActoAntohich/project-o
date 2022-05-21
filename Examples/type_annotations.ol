class Main is
    this is
        // Explicit type annotation
        var list1: List<Integer> = []
        list1.Append(20)

        // Type is inferred here
        var list2 = [1, 2, 3]
        
        IO().Write(
            list2.Get(2)
                .Plus(
                list1.Get(
                    list1.Length().Minus(1)
                )
            ).ToString()
        )

        // This should print "11"
        IO().Write([1, 2].Get(0).Plus(10).ToString())
    end
end