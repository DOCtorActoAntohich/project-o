class Main is
	this() is
        var baka = Imposter()
	end
end

class Imposter is
    var name = "Baka"
    var age: Integer
    
    this() is
        var result: String = this.name.Concatenate(" of age ").Concatenate(this.age.ToString())
        IO().WriteLine(result)
    end
end