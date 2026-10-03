namespace Api.Models;

// TODO(you): a CheckIn means "habit X was done on day Y".
// Use Habit.cs as a reference. You need:
//   - Id (int)
//   - HabitId (int)      -> foreign key; EF links it to Habit by the name
//   - Habit (Habit)      -> navigation property; hint: `public Habit Habit { get; set; } = null!;`
//   - Date (DateOnly)    -> just a day, no time; this is what streaks are calculated from
//
// Then: uncomment CheckIns in Habit.cs and the DbSet in AppDbContext.cs.

public class CheckIn
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public Habit Habit { get; set; } = null!;

    public DateOnly Date { get; set; }
}