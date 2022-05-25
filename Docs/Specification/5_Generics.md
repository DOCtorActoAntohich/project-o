# Generics

The O language also features a support of [generic](https://en.wikipedia.org/wiki/Generic_programming) classes, which allow to specify the types of the objects used inside these classes, during the instantiation process, i.e. when the object of the generic class is created.

## Generic class definition

To create a generic class, just list the names of generic types to be used inside the class field definitions and methods in the angle brackets after the class name.

## Using the generic types

To use the defined generic type, just pass it where needed, the same way as usual types (`String`, `Real`, etc.) would be passed.

## Example of a generic class

```ts
class Pair<Type1, Type2> is  // 2 generic types used
    var field1 : Type1
    var field2 : Type2

    this(item1: Type1, item2: type2) is
        this.field1 = item1
        this.field2 = item2
    end

    method First() : Type1 is
        return this.field1
    end

    method Second() : Type2 is
        return this.field2
    end
end
```
