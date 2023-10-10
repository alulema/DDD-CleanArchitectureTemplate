using CleanDds.Common.Numbers;
using FluentAssertions;
using Xunit;

namespace CleanDds.Testing
{
    public class MathTests
    {
        [Fact]
        public void RoundHalfToEven_ShouldRoundCorrectly()
        {
            // Act
            var result1 = Util.RoundHalfToEven(23.5m);
            var result2 = Util.RoundHalfToEven(24.5m);
            var result3 = Util.RoundHalfToEven(-23.5m);
            var result4 = Util.RoundHalfToEven(-24.5m);

            // Assert
            result1.Should().Be(24m, because: "23.5 should round to the nearest even number");
            result2.Should().Be(24m, because: "24.5 should round down to the nearest even number");
            result3.Should().Be(-24m, because: "-23.5 should round to the nearest even number");
            result4.Should().Be(-24m, because: "-24.5 should round down to the nearest even number");
        }
    }
}