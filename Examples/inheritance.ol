class Main is
    this is
        var cat = SuperCat("James", "LaserEyes")
        
        IO().WriteLine(cat.Fight())
    end
end

class Animal is
    var name = String()
    
    this (name: String) is
        this.name = name
    end
    
    method Sound() : String is
        return "Unknown"
    end
end


class Cat extends Animal is
    this (name: String) is
        base(name)
    end

    method Sound() : String is
        return "Meow"
    end
end


class SuperCat extends Cat is
    var ability = String()

    this (name: String, ability: String) is
        base(name)
        this.ability = ability
    end

    method Fight() : String is
        return String("Cat ").Concatenate(
            this.name
        ).Concatenate(
            " uses ability "
        ).Concatenate(
            this.ability
        ).Concatenate(
            " and says "
        ).Concatenate(
            this.Sound()
        )
    end
end