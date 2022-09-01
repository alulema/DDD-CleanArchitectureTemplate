using System;

namespace CleanDds.Common.Numbers;

public static class Util
{
    public static decimal RoundHalfToEven(decimal value)
    {
        var decPart = Math.Abs(value % 1);
        var intPart = Math.Abs(value) - decPart;

        var nextNumber = Math.Abs(intPart) + 1;
        var previousNumber = Math.Abs(intPart);

        if (decPart == 0.5m)
        {
            if (nextNumber % 2 == 0)
                return nextNumber * (value > 0 ? 1 : -1);

            if (previousNumber % 2 == 0)
                return previousNumber * (value > 0 ? 1 : -1);
        }

        return value;
    }
}
