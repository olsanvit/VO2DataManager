using FluentAssertions;

namespace VO2DataManager.Tests;

/// <summary>
/// Tests duration formatting: seconds -> human-readable string.
/// Format: M:SS for under 1 hour, H:MM:SS for 1 hour+.
/// The Song model stores no duration field; these tests verify the formatting
/// logic that would appear in a service or helper.
/// </summary>
public class SongDurationTests
{
    // ── Duration formatter (inline helper mirroring standard music app behavior) ──

    private static string FormatDuration(int totalSeconds)
    {
        if (totalSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(totalSeconds), "Duration cannot be negative.");

        var ts = TimeSpan.FromSeconds(totalSeconds);

        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}";

        return $"{ts.Minutes}:{ts.Seconds:D2}";
    }

    [Fact]
    public void FormatDuration_ZeroSeconds_Returns_0Colon00()
    {
        FormatDuration(0).Should().Be("0:00");
    }

    [Fact]
    public void FormatDuration_65Seconds_Returns_1Colon05()
    {
        FormatDuration(65).Should().Be("1:05");
    }

    [Fact]
    public void FormatDuration_3661Seconds_Returns_1Colon01Colon01()
    {
        FormatDuration(3661).Should().Be("1:01:01");
    }

    [Fact]
    public void FormatDuration_59Seconds_Returns_0Colon59()
    {
        FormatDuration(59).Should().Be("0:59");
    }

    [Fact]
    public void FormatDuration_3600Seconds_Returns_1Colon00Colon00()
    {
        FormatDuration(3600).Should().Be("1:00:00");
    }

    [Fact]
    public void FormatDuration_7322Seconds_Returns_2Colon02Colon02()
    {
        // 7322 = 2*3600 + 2*60 + 2
        FormatDuration(7322).Should().Be("2:02:02");
    }

    [Fact]
    public void FormatDuration_NegativeDuration_ThrowsArgumentOutOfRangeException()
    {
        var act = () => FormatDuration(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0, "0:00")]
    [InlineData(60, "1:00")]
    [InlineData(90, "1:30")]
    [InlineData(599, "9:59")]
    [InlineData(600, "10:00")]
    [InlineData(3599, "59:59")]
    public void FormatDuration_VariousUnderOneHour_CorrectFormat(int seconds, string expected)
    {
        FormatDuration(seconds).Should().Be(expected);
    }
}
