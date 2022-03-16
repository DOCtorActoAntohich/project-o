class Main is
  var sus : true
  this is
    this.PrintSus
    IO.Write("this".Concatenate(" is ").Concatenate(OtherClass.somestring))
    IO.Write(OtherClass.GetCoolNumber.ToString())
    this.sus := false
  end

  method PrintSus is
    var res : OtherClass.GetSusValue
    IO.Write(res.ToString())
  end
end

class OtherClass is
  var unusedfield : "cool language"
  var somestring   : "weird language"
  var VERY_Bad_in_t_e_g_e_r : Integer(5).Plus(Integer(7)).Plus(Integer(8))

  method GetCoolNumber : Integer is
    var a : 994
    var b : 925
    var c : 900
    c := a.Plus(b)
    return a.Mod(b)
  end

  method GetSusValue : Boolean is
    return Main.sus
  end
end
