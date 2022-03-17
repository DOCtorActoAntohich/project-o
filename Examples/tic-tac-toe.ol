class Random is 
    var K : 2129
    var B : 5077
    var Mod : 9343
    var value : 0
    
    this is 
        this.value := Time.Current().Mod(this.Mod)
    end
    
    this (seed: Integer) is 
        this.value := seed
    end
    
    method RandInt(from: Integer, to: Integer) : Integer is
         this.value := this.value.Mult(this.K).Plus(this.B).Mod(this.Mod)
         return from.Plus(this.value.Mod(to.Minus(from).Plus(1)))
    end
end


class TicTacToe is
    // Constants.
    var Zero : 1
    var Cross : 2
    
    // Game field.
    var cell11 : 0
    var cell12 : 0
    var cell13 : 0
    var cell21 : 0
    var cell22 : 0
    var cell23 : 0
    var cell31 : 0
    var cell32 : 0
    var cell33 : 0
    
    // AI hard level.
    var level : 0
    var random : Random
    
    this is
        this.PrintGuideline()
        
        // Ask hard level.
        while this.level.Greater(0).And(this.level.LessEqual(5)).Not() loop
            IO.Write("Choose difficulty level (1-5) ") 
            this.level := IO.ReadLine().ToInteger()
        end
        
        // Ask first move.
        var answer : ""
        while answer.Equal("y").Or(answer.Equal("n")).Not() loop
            IO.Write("Do you want to be first? (y/n) ") 
            answer := IO.ReadLine() 
        end   
        
        // Roles.
        var user : this.Zero
        var computer : this.Cross
        if answer.Equal("y") then
            user := this.Cross
            computer := this.Zero
            this.PrintGameField()
        end

        // Current player.
        var player : this.Cross
        
        // Game loop.
        while this.CheckIfFinish().Or(this.CheckIfWin().Equal(0).Not()).Not() loop  
            if player.Equal(user) then
                this.MakeUserMove(player)
                player := computer
            else
                this.MakeComputerMove(player)
                player := user
            end
            
            this.PrintGameField()
        end
        
        var winner : this.CheckIfWin()
        if winner.Equal(0) then 
            IO.WriteLine("Draw!")
        else
            if winner.Equal(user) then
                IO.WriteLine("You win!")
            else
                IO.WriteLine("You lose!")
            end
        end
    end

    method MakeUserMove(player: Integer) : Void is
        IO.Write("Your move! Type cell and press Enter: ")
        var move : IO.ReadLine().ToInteger()
        
        // Read while not correct.
        while this.GetCellValue(move).Equal(0).Not() loop
            IO.Write("Your move! Type cell and press Enter: ")
            move := IO.ReadLine().ToInteger()
        end
        
        this.SetCellValue(move, player)
    end
    
    method MakeComputerMove(player: Integer) : Void is
        if this.CheckIfWin().Equal(0).Not() then
            IO.WriteLine("Already win!")
            return
        end
    
        if this.random.RandInt(1, 5).GreaterEqual(this.level) then
            IO.WriteLine("Random")
            
            var row : this.random.RandInt(1, 3)
            var column : this.random.RandInt(1, 3)
            
            while this.GetCellValue(row.Mult(10).Plus(column)).Equal(0).Not() loop
                row := this.random.RandInt(1, 3)
                column := this.random.RandInt(1, 3)
            end
            
            this.SetCellValue(row.Mult(10).Plus(column), player)
            return
        end
    
        var bestMove : 0
        // Cross player.
        var bestScore : 1000
        // Zero player.
        if player.Equal(this.Zero) then
            bestScore := Integer.Min
        end
        
        var row : 1        
        while row.LessEqual(3) loop
            var column : 1
            while column.LessEqual(3) loop
                var move : row.Mult(10).Plus(column)
                
                // Cell is clear. Can make move.
                if this.GetCellValue(move).Equal(0) then
                    // Make move.
                    this.SetCellValue(move, player)
                    
                    // Cross player, minimizing.
                    if player.Equal(this.Cross) then
                        // Estimate.
                        var score : this.MinMax(this.Zero, 0)
                        
                        // Minimize.
                        if score.LessEqual(bestScore) then
                            bestMove := move
                            bestScore := score
                        end
                        
                    // Zero player, maximizing.
                    else
                        // Estimate.
                        var score : this.MinMax(this.Cross, 0)
                        
                        // Maximize.
                        if score.GreaterEqual(bestScore) then
                            bestMove := move
                            bestScore := score
                        end
                    end
                    
                    // Revert move.
                    this.SetCellValue(move, 0)
                end
                
                column := column.Plus(1)           
            end
            
            row := row.Plus(1) 
        end
        
        if bestMove.Equal(0) then
            IO.WriteLine("Cannot perform move!")
            return
        end
        
        // Move.
        this.SetCellValue(bestMove, player)
    end
    
    // Cross player tries to minimize score and Zero player tries to maximize score.
    method MinMax(player: Integer, depth: Integer) : Integer is
        var winner : this.CheckIfWin()
        
        // Cross won.
        if winner.Equal(this.Cross) then
            // Minimize.
            return depth.Minus(10)
        end
        
        // Zero won.
        if winner.Equal(this.Zero) then
            // Maximize.
            return Integer(10).Minus(depth)            
        end
        
        // Cross player.
        var bestScore : 1000
        // Zero player.
        if player.Equal(this.Zero) then
            bestScore := Integer.Min
        end
    
        var row : 1        
        while row.LessEqual(3) loop
            var column : 1
            while column.LessEqual(3) loop
                var move : row.Mult(10).Plus(column)
 
                // Cell is clear. Can make move.
                if this.GetCellValue(move).Equal(0) then
                    // Make move.
                    this.SetCellValue(move, player)
                    
                    // Cross player, minimizing.
                    if player.Equal(this.Cross) then
                        // Estimate.
                        var score : this.MinMax(this.Zero, depth.Plus(1))
                        
                        // Minimize.
                        if score.LessEqual(bestScore) then
                            bestScore := score
                        end
                        
                    // Zero player, maximizing.
                    else
                        // Estimate.
                        var score : this.MinMax(this.Cross, depth.Plus(1))
                        
                        // Maximize.
                        if score.GreaterEqual(bestScore) then
                            bestScore := score
                        end
                    end

                    // Revert move.
                    this.SetCellValue(move, 0)
                end
                
                column := column.Plus(1)
            end
            
            row := row.Plus(1)
        end
        
        // No moves available.
        if bestScore.Equal(1000).Or(bestScore.Equal(Integer.Min)) then 
            return 0
        end
        
        return bestScore
    end
    
    method PrintGameField() : Void is
        this.PrintCellValue(this.cell11)
        IO.Write("|")
        this.PrintCellValue(this.cell12)
        IO.Write("|")
        this.PrintCellValue(this.cell13)
        IO.WriteLine("") 
        
        IO.WriteLine("-+-+-")
        
        this.PrintCellValue(this.cell21)
        IO.Write("|")
        this.PrintCellValue(this.cell22)
        IO.Write("|")
        this.PrintCellValue(this.cell23)    
        IO.WriteLine("") 
    
        IO.WriteLine("-+-+-")
        
        this.PrintCellValue(this.cell31)
        IO.Write("|")
        this.PrintCellValue(this.cell32)
        IO.Write("|")
        this.PrintCellValue(this.cell33)
        
        IO.WriteLine("") 
        IO.WriteLine("") 
        
    end
    
    method PrintCellValue(value: Integer) : Void is
        if value.Equal(this.Cross) then
            IO.Write("X")
            return
        end
        
        if value.Equal(this.Zero) then
            IO.Write("O")
            return
        end
        
        IO.Write(" ")
    end
    
    method PrintGuideline() : Void is
        IO.WriteLine("Welcome to Tic-Tac-Toe v1.0!")
        IO.WriteLine("Map of the game field: ")
        IO.WriteLine("")
        IO.WriteLine("11|12|13")
        IO.WriteLine("--+--+--")
        IO.WriteLine("21|22|23")
        IO.WriteLine("--+--+--")
        IO.WriteLine("31|32|33") 
        IO.WriteLine("")
        IO.WriteLine("Good luck!")
        IO.WriteLine("")
        IO.WriteLine("")
    end
    
    method GetCellValue(cell: Integer) : Integer is
        if cell.Equal(11) then
            return this.cell11
        end
        
        if cell.Equal(12) then
            return this.cell12
        end
        
        if cell.Equal(13) then
            return this.cell13
        end
        
        if cell.Equal(21) then
            return this.cell21
        end
        
        if cell.Equal(22) then
            return this.cell22
        end
        
        if cell.Equal(23) then
            return this.cell23
        end
        
        if cell.Equal(31) then
            return this.cell31
        end
        
        if cell.Equal(32) then
            return this.cell32
        end
        
        if cell.Equal(33) then
            return this.cell33
        end
        
        // Error.
        IO.WriteLine("Invalid cell!")
        return 3
    end
    
    method SetCellValue(cell: Integer, value: Integer) : Void is
        if value.Equal(this.Cross).Or(value.Equal(this.Zero)).Or(value.Equal(0)).Not() then
            IO.WriteLine("Invalid player!")
            return
        end
        
        if cell.Equal(11) then
            this.cell11 := value
            return
        end
        
        if cell.Equal(12) then
            this.cell12 := value
            return
        end
        
        if cell.Equal(13) then
            this.cell13 := value
            return
        end
        
        if cell.Equal(21) then
            this.cell21 := value
            return
        end
        
        if cell.Equal(22) then
            this.cell22 := value
            return
        end
        
        if cell.Equal(23) then
            this.cell23 := value
            return
        end
        
        if cell.Equal(31) then
            this.cell31 := value
            return
        end
        
        if cell.Equal(32) then
            this.cell32 := value
            return
        end
        
        if cell.Equal(33) then
            this.cell33 := value
            return
        end
        
        // Error.
        IO.WriteLine("Invalid cell!")
    end
    
    method CheckIfFinish() : Boolean is
        var row : 1        
        while row.LessEqual(3) loop
            var column : 1
            while column.LessEqual(3) loop
                var move : row.Mult(10).Plus(column)
                
                if this.GetCellValue(move).Equal(0) then
                    return false
                end
                
                column := column.Plus(1)
            end
            
            row := row.Plus(1)
        end
        
        return true
    end
    
    method CheckIfWin() : Integer is
        // Horizontal.
        if this.cell11.Equal(this.cell12).And(this.cell12.Equal(this.cell13)).And(this.cell11.Equal(0).Not()) then
            return this.cell11
        end
        
        // Horizontal.
        if this.cell21.Equal(this.cell22).And(this.cell22.Equal(this.cell23)).And(this.cell21.Equal(0).Not()) then
            return this.cell21
        end
        
        // Horizontal.
        if this.cell31.Equal(this.cell32).And(this.cell32.Equal(this.cell33)).And(this.cell31.Equal(0).Not()) then
           return this.cell31
        end
        
        
        // Vertical.
        if this.cell11.Equal(this.cell21).And(this.cell21.Equal(this.cell31)).And(this.cell11.Equal(0).Not()) then
            return this.cell11
        end
        
        // Vertical.
        if this.cell12.Equal(this.cell22).And(this.cell22.Equal(this.cell32)).And(this.cell12.Equal(0).Not()) then
            return this.cell12
        end
        
        // Vertical.
        if this.cell13.Equal(this.cell23).And(this.cell23.Equal(this.cell33)).And(this.cell13.Equal(0).Not()) then
            return this.cell13
        end
        
        
        // Diagonal.
        if this.cell11.Equal(this.cell22).And(this.cell22.Equal(this.cell33)).And(this.cell11.Equal(0).Not()) then
            return this.cell11
        end
        
        // Diagonal.
        if this.cell13.Equal(this.cell22).And(this.cell22.Equal(this.cell31)).And(this.cell13.Equal(0).Not()) then
            return this.cell13
        end
        
        return 0
    end
end
