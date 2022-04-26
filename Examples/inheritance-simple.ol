class Main is
	this is
		var somePhone = NotMyPhone
		somePhone.Turn_on()
	end
end

class Phone is 
	var serial = 0

	method Turn_on() : Void is
		if this.serial.Equal(0).Not then 
			IO.Write("Turn on the phone with number ")
			IO.WriteLine(this.serial.ToString())
		else
			IO.WriteLine("It's the basic phone, cannot turn it on")
		end
	end
end

class IPhone extends Phone is
	this is
		// basic serial number for all iPhones
		this.serial = 700000000
	end
end

class NotMyPhone extends IPhone is
	this is
		this.serial = this.serial.Plus(123456)
	end
end