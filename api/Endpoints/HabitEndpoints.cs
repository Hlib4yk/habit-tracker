using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

// Request/response shapes. `record` = immutable data class, great for DTOs.
public record CreateHabitRequest(string Name, string? Description);
public record HabitResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int CurrentStreak,
    int LongestStreak,
    List<DateOnly> CheckIns);
public record UpdateHabitRequest(string Name, string? Description);

public static class HabitEndpoints
{
    // An extension method: lets Program.cs call `app.MapHabitEndpoints()`.
    // Similar to mounting an Express Router.
    public static void MapHabitEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/habits");

        // Handler parameters are filled in automatically:
        // services (AppDbContext) come from DI, the JSON body is deserialized into the request record.
        group.MapGet("/", async (AppDbContext db) =>
        {
            // Include() = load related CheckIns in the same query (like Prisma's `include`).
            // Without it, habit.CheckIns would be an empty list.
            var habits = await db.Habits
                .Include(h => h.CheckIns)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();

            // Streaks are computed in C# after loading, so ToResponse runs in memory, not in SQL.
            return Results.Ok(habits.Select(ToResponse));
        });

        group.MapPost("/", async (CreateHabitRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Name is required");

            var habit = new Habit { Name = request.Name.Trim(), Description = request.Description };
            db.Habits.Add(habit);
            await db.SaveChangesAsync();

            return Results.Created($"/api/habits/{habit.Id}", ToResponse(habit));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var habit = await FindWithCheckIns(db, id);

            if (habit is null)
                return Results.NotFound();

            // Return the DTO, not the EF entity, so every endpoint has the same JSON shape.
            return Results.Ok(ToResponse(habit));
        });

        // Three parameters, each bound automatically:
        // `id` from the route, `request` from the JSON body, `db` from DI.
        group.MapPut("/{id:int}", async (int id, UpdateHabitRequest request, AppDbContext db) =>
        {
            var habit = await FindWithCheckIns(db, id);

            if (habit is null)
                return Results.NotFound();

            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Name is required");

            // No explicit update() call: EF tracks the loaded entity
            // and SaveChangesAsync() issues an UPDATE for the changed properties.
            habit.Name = request.Name.Trim();
            habit.Description = request.Description;
            await db.SaveChangesAsync();

            return Results.Ok(ToResponse(habit));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var habit = await db.Habits.FindAsync(id);

            if (habit is null)
                return Results.NotFound();

            db.Habits.Remove(habit);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // Mark a habit as done on a given day, e.g. PUT /api/habits/1/checkins/2026-10-04.
        // PUT (not POST) because it's idempotent: calling it twice leaves the same state.
        // Both check-in endpoints return the updated habit, so the UI can re-render from one response.
        group.MapPut("/{id:int}/checkins/{date}", async (int id, DateOnly date, AppDbContext db) =>
        {
            var habit = await FindWithCheckIns(db, id);

            if (habit is null)
                return Results.NotFound();

            if (date > Today())
                return Results.BadRequest("Cannot check in for a future date");

            // Already checked in -> nothing to do. Without this, the unique index
            // on (HabitId, Date) would make SaveChanges throw.
            if (!habit.CheckIns.Any(c => c.Date == date))
            {
                habit.CheckIns.Add(new CheckIn { Date = date });
                await db.SaveChangesAsync();
            }

            return Results.Ok(ToResponse(habit));
        });

        group.MapDelete("/{id:int}/checkins/{date}", async (int id, DateOnly date, AppDbContext db) =>
        {
            var habit = await FindWithCheckIns(db, id);

            if (habit is null)
                return Results.NotFound();

            // FirstOrDefault = JS `find()`: the first match or null.
            var checkIn = habit.CheckIns.FirstOrDefault(c => c.Date == date);
            if (checkIn is not null)
            {
                habit.CheckIns.Remove(checkIn);
                await db.SaveChangesAsync();
            }

            return Results.Ok(ToResponse(habit));
        });
    }

    // FindAsync can't Include related data, so we query by Id instead.
    private static Task<Habit?> FindWithCheckIns(AppDbContext db, int id) =>
        db.Habits.Include(h => h.CheckIns).FirstOrDefaultAsync(h => h.Id == id);

    // Server-local date. Good enough for a single-user pet project;
    // a real app would use the user's time zone.
    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);

    private static HabitResponse ToResponse(Habit h)
    {
        var dates = h.CheckIns.Select(c => c.Date).Order().ToList();
        var streak = StreakCalculator.Calculate(dates, Today());

        return new(h.Id, h.Name, h.Description, h.CreatedAt, streak.Current, streak.Longest, dates);
    }
}
