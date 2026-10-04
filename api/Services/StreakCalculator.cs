using System.Globalization;

namespace Api.Services;

public record StreakResult(int Current, int Longest);

public static class StreakCalculator
{
    public static StreakResult Calculate(IEnumerable<DateOnly> checkInDates, DateOnly today)
    {
        var set = checkInDates.ToHashSet();
        var day = set.Contains(today) ? today : today.AddDays(-1);
        var current = 0;

        while (set.Contains(day))
        {
            current++;
            day = day.AddDays(-1);
        }

        var sorted = set.Order();
        var longest = 0;
        var run = 0;
        DateOnly? prev = null;

        foreach (var d in sorted)
        {
            if (prev != null && d == prev.Value.AddDays(1))
                run++;
            else run = 1;
            longest = Math.Max(longest, run);
            prev = d;
        }
        return new StreakResult(current, longest);
    }
}
