namespace Api.Models;

// An EF Core entity: a plain class, one row in the "Habits" table.
// Property named `Id` becomes the primary key by convention.
public class Habit
{
    public int Id { get; set; }

    // `required` = must be set when creating the object (like a non-optional field in TS).
    public required string Name { get; set; }

    // `string?` = nullable, like `description?: string` in TS.
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CheckIn> CheckIns { get; set; } = [];
}
