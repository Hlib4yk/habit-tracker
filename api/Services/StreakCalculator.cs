namespace Api.Services;

public record StreakResult(int Current, int Longest);

public static class StreakCalculator
{
    // Rules (the tests in api.Tests/StreakCalculatorTests.cs are the full spec):
    //   - Current streak: consecutive days ending today.
    //     If today has no check-in yet, the streak is still alive if yesterday has one
    //     (the day isn't over, so you haven't broken it yet).
    //   - Longest streak: the longest run of consecutive days ever.
    //   - Input may be unsorted and contain duplicates.
    public static StreakResult Calculate(IEnumerable<DateOnly> checkInDates, DateOnly today)
    {
        // TODO(you): make the tests pass.
        // Useful tools:
        //   - checkInDates.ToHashSet()          -> dedupe + O(1) Contains, like `new Set(dates)` in JS
        //   - .Order() / .Distinct() / .ToList() -> LINQ, like array methods in JS
        //   - date.AddDays(-1)                   -> previous day (DateOnly is immutable, returns a new value)
        //   - Math.Max(a, b)
        throw new NotImplementedException();
    }
}
