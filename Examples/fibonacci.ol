class Main is
    method positive(num: Integer) : Boolean is
        return num.Greater(0)
    end
    method succ(num: Integer) : Integer is
        return num.Plus(1)
    end
    method pred(num: Integer) : Integer is
        if this.positive(num).Not() then
            return 0
        end
        return num.Minus(1)
    end
    method fib(num: Integer) : Integer is
        if num.Less(2) then
            return 1
        end

        return this.fib(this.pred(num)).Plus(
            this.fib(this.pred(this.pred(num)))
        )
    end
    this() is
        IO().Write("You need to pass an Integer as an argument")
    end
    this(N: Integer) is
        IO().Write("Fibonacci at index ".Concatenate(N.ToString()).Concatenate(": "))
        IO().Write(this.fib(N).ToString())
    end
end