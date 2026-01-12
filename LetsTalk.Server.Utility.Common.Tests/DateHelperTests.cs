using FluentAssertions;

namespace LetsTalk.Server.Utility.Common.Tests;

[TestFixture]
public class DateHelperTests
{
    [Test]
    public void GetUnixTimestamp_ShouldReturnPositiveValue()
    {
        // Act
        var result = DateHelper.GetUnixTimestamp();

        // Assert
        result.Should().BePositive();
    }

    [Test]
    public void GetUnixTimestamp_ShouldReturnReasonableCurrentTimestamp()
    {
        // Arrange
        var expectedMinTimestamp = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
        var expectedMaxTimestamp = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();

        // Act
        var result = DateHelper.GetUnixTimestamp();

        // Assert
        result.Should().BeGreaterThan(expectedMinTimestamp);
        result.Should().BeLessThan(expectedMaxTimestamp);
    }

    [Test]
    public void GetUnixTimestamp_CalledTwiceWithDelay_ShouldReturnDifferentValues()
    {
        // Act
        var first = DateHelper.GetUnixTimestamp();
        Thread.Sleep(1100); // Sleep for more than 1 second to ensure different timestamps
        var second = DateHelper.GetUnixTimestamp();

        // Assert
        second.Should().BeGreaterThan(first);
        (second - first).Should().BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public void GetUnixTimestamp_ShouldMatchBuiltInCalculation()
    {
        // Act
        var helperResult = DateHelper.GetUnixTimestamp();
        var builtInResult = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Assert - Allow 1 second difference due to timing
        Math.Abs(helperResult - builtInResult).Should().BeLessThanOrEqualTo(1);
    }
}