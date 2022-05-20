class Main is
    this is
        var list: List<Dictionary<String, Integer>> = []
        list.Append({})
        list.Get(0).Set("sus", 42)
        IO().WriteLine(list.Get(0).Search(42))
    end
end