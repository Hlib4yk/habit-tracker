using Api.Services;

namespace Api.Tests;

// xUnit cheat sheet (vs Jest):
//   [Fact]                 -> test("...")
//   [Theory] + [InlineData] -> test.each([...])
//   Assert.Equal(expected, actual) -> expect(actual).toBe(expected)   (note: expected goes FIRST)
public class StreakCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 4);

    // Helper: D(0) = today, D(1) = yesterday, D(2) = two days ago...
    private static DateOnly D(int daysAgo) => Today.AddDays(-daysAgo);

    private static StreakResult Calc(params int[] daysAgo) =>
        StreakCalculator.Calculate(daysAgo.Select(D), Today);

    [Fact]
    public void No_check_ins_gives_zero_streaks()
    {
        var result = Calc();

        Assert.Equal(new StreakResult(0, 0), result);
    }

    [Fact]
    public void Only_today_gives_streak_of_one()
    {
        Assert.Equal(new StreakResult(1, 1), Calc(0));
    }

    [Fact]
    public void Consecutive_days_ending_today_are_counted()
    {
        Assert.Equal(new StreakResult(3, 3), Calc(0, 1, 2));
    }

    [Fact]
    public void Streak_is_still_alive_if_today_is_not_checked_in_yet()
    {
        // Yesterday and the day before — today isn't over, so the streak isn't broken.
        Assert.Equal(new StreakResult(2, 2), Calc(1, 2));
    }

    [Fact]
    public void Streak_is_broken_after_a_missed_day()
    {
        // Last check-in was two days ago -> yesterday was missed.
        Assert.Equal(new StreakResult(0, 2), Calc(2, 3));
    }

    [Fact]
    public void A_gap_resets_the_current_streak()
    {
        // Today, yesterday, (gap at 2), 3, 4, 5
        Assert.Equal(new StreakResult(2, 3), Calc(0, 1, 3, 4, 5));
    }

    [Fact]
    public void Longest_streak_can_be_in_the_past()
    {
        // A 5-day run a month ago, then only today.
        Assert.Equal(new StreakResult(1, 5), Calc(0, 30, 31, 32, 33, 34));
    }

    [Fact]
    public void Unsorted_input_and_duplicates_are_handled()
    {
        Assert.Equal(new StreakResult(3, 3), Calc(2, 0, 1, 0, 2, 1));
    }

    [Theory]
    [InlineData(new[] { 0 }, 1)]
    [InlineData(new[] { 0, 1, 2, 3, 4, 5, 6 }, 7)]
    [InlineData(new[] { 0, 2, 4, 6 }, 1)]
    public void Longest_streak_examples(int[] daysAgo, int expectedLongest)
    {
        Assert.Equal(expectedLongest, Calc(daysAgo).Longest);
    }
}
