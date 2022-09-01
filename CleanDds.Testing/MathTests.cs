using System;
using CleanDds.Common.Numbers;
using Xunit;

namespace CleanDds.Testing;

public class MathTests
{
    [Fact]
    public void TestRoundHalfToEven()
    {
        var result1 = Util.RoundHalfToEven(23.5m);
        Assert.Equal(24m, result1);
        var result2 = Util.RoundHalfToEven(24.5m);
        Assert.Equal(24m, result2);
        var result3 = Util.RoundHalfToEven(-23.5m);
        Assert.Equal(-24m, result3);
        var result4 = Util.RoundHalfToEven(-24.5m);
        Assert.Equal(-24m, result4);
    }
}