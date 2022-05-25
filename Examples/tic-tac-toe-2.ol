class Field is

    var data = [0,0,0,0,0,0,0,0,0]
    
    method Print(): Void is
        row = 0
        while row.Less(3) loop
            column = 0
            while column.Less(3) loop
                index = row.Times(3).Plus(column)
                IO().Write(data.Get(index).toString())
                IO().Write(" ")
                column = column.Plus(1)
            end
            IO().WriteLine("")
            row = row.Plus(1)
        end
    end

    method GetWinner(): Integer is
        possible_wins = [
            [0, 1, 2], [3, 4, 5], [6, 7, 8], 
            [0, 3, 6], [1, 4, 7], [2, 5, 8], 
            [0, 4, 8], [2, 4, 6],
        ]
        option_index = 0
        while option_index.Less(possible_wins.Length()) loop
            option = possible_wins.Get(option_index)
            a = this.data.Get(option.Get(0))
            b = this.data.Get(option.Get(1))
            c = this.data.Get(option.Get(2))
            if a.Equals(b).And(a.Equals(c)) then
                return a
            end
            option_index = option_index.Plus(1)
        end
        return 0
    end

    method IsFilled(): Boolean is
        return this.data.Sum().GreaterEqual(9)
    end

    method SetValue(index: Integer, value: Integer): Void is
        if this.data.Get(index).Equals(0).Not() then
            IO().WriteLine("error: set already existing value to field")
        end
        this.data.Set(index, value)
    end

    


end