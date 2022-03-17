class Main is
    this is
        var n : Newbie(7)
    end
end

class Newbie is
    var age : 0
  
    this(age: Integer) is
        this.age := age.Plus(1)
    end
  
    method gScore() : Real is
        return this.age.ToReal.Mult(this.gregificationStage())
    end
    
    method gregificationStage() : Real is
        return 0.5
    end
end


class Imposter extends Newbie is
    var foxiness: 0

    this(age: Integer, foxiness: Integer) is
        base(age)
        this.foxiness := foxiness
    end
  
    method doubleAge() is
        if this.isFoxyEnough then
            this.age := this.age.Mult(2)
        end
    end
  
    method gScore() : Real is
        if this.isFoxyEnough() then
            return this.age.ToReal
        else
            return this.age.ToReal.Mult(this.gregificationStage)
        end
    end
    
    method gregificationStage() : Real is
        return 0.65
    end
  
    method isFoxyEnough() : Boolean is
        return this.foxiness.GreaterEqual(3)
    end
end


class SmartGuy extends Newbie is
    var isBigBranes : false
    var susness : 0
    
    this(age: Integer, susness: Integer, bigBranes: Boolean) is
        base(age)
        this.susness := susness
        this.isBigBranes := bigBranes
    end
    
    method beEpic() is
        IO.WriteLine("Smart guy is being epic again...")
    end
    
    method gScore() : Real is
        return this.age.ToReal.Mult(this.gregificationStage)
    end
    
    method gregificationStage() : Real is
        return 7.0 // By this time they usually reach Gregification Stage 7.
    end
end

class GregoriusTechneticies extends SmartGuy is
    this(age: Integer) is
        base(age, 0, true)
    end
    
    method beEpic(howEpic: Integer) is
        var count : Integer(howEpic)
        while count.Greater(0) loop
            IO.WriteLine("Greg.")
            count := count.Minus(1)
        end
    end
    
    method gScore() : Real is
        return 999999.999999
    end
    
    method gregificationStage() : Real is
        return 10.0
    end
end