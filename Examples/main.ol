class Main extends OtherClass is
  var sus : true
  this is
    IO.Write("this".Concatenate(" is ").Concatenate(this.somestring))
    IO.Write(this.GetCoolNumber(994, 925).ToString())
    this.sus := false
  end
end

class OtherClass is
  var unusedfield : "cool language"
  var somestring   : "weird language"

  this(new: String) is
    this.somestring := new
  end

  method GetCoolNumber(a: Integer, b: Integer) : Integer is
    return a.Mod(b)
  end
end

class What extends OtherClass is
  this(new: String) is
    base(new)
  end
end