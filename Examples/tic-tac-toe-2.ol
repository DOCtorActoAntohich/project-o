class Field is
    var EMPTY = 0
    var CIRCLE = 1
    var CROSS = 2
    var data: List<Integer>
    
    this() is
        this.data = [
            this.EMPTY, this.EMPTY, this.EMPTY,
            this.EMPTY, this.EMPTY, this.EMPTY,
            this.EMPTY, this.EMPTY, this.EMPTY,
        ]
    end

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
            if a.Equal(b).And(a.Equal(c)) then
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
        if this.data.Get(index).Equal(0).Not() then
            IO().WriteLine("error: set already existing value to field")
        end
        this.data.Set(index, value)
    end
end


class TicTacToe is
    var field: Field
    var user: Integer
    var computer: Integer
    
    this() is 
        this.field = Field()
    end

    method DecideRoles(): Void is
        this.user = this.CROSS
        this.computer = this.CIRCLE
    end

    method Run(): Void is
        this.DecideRoles()
        var currentPlayer = this.CROSS

        while this.Finished().Not() loop
            if currentPlayer.Equal(this.user) then
                this.MakeUserMove()
                currentPlayer = this.computer
            else
                this.MakeComputerMove()
                currentPlayer = this.user
            end

            this.field.Print()
        end

        var winner = this.field.GetWinner()
        var output_messages = {
            0: "Draw!",
            this.user: "You won!",
            this.computer: "You loose!",
        }
        IO().WriteLine(output_messages.Get(winner))
    end

    method Finished(): Boolean is
        return this.field.IsFilled().Or(
               this.field.GetWinner().Equal(0).Not()
        )
    end

    method MakeUserMove(): Void is
        IO().Write("Your move! Type cell and press Enter: ")
        var move = IO().ReadLine().ToInteger()
        this.field.SetValue(move, this.user)
    end

    method MakeComputerMove(): Void is
        var move = this.AIMove()
        this.field.SetValue(move, this.computer)
    end

    method AIMove(): Integer is
        if this.Finished() then
            return -1
        end

        var i = 0
        while i.Less(9) loop
            if this.field.data.Get(i).Equal(this.field.EMPTY) then
                return i
            end
            i = i.Plus(1)
        end
    end
end

class Main is
    this is
        game = TicTacToe()
        game.Run()
    end
end