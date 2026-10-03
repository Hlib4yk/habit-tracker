using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

// Request/response shapes. `record` = immutable data class, great for DTOs.
public record CreateHabitRequest(string Name, string? Description);
public record HabitResponse(int Id, string Name, string? Description, DateTime CreatedAt);

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
            var habits = await db.Habits
                .OrderBy(h => h.CreatedAt)
                .Select(h => new HabitResponse(h.Id, h.Name, h.Description, h.CreatedAt))
                .ToListAsync();

            return Results.Ok(habits);
        });

        group.MapPost("/", async (CreateHabitRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Name is required");

            var habit = new Habit { Name = request.Name.Trim(), Description = request.Description };
            db.Habits.Add(habit);
            await db.SaveChangesAsync();

            var response = new HabitResponse(habit.Id, habit.Name, habit.Description, habit.CreatedAt);
            return Results.Created($"/api/habits/{habit.Id}", response);
        });

        // TODO(you): GET /api/habits/{id}
        //   Route params: group.MapGet("/{id:int}", async (int id, AppDbContext db) => ...)
        //   Use db.Habits.FindAsync(id); return Results.NotFound() if null.

        // TODO(you): PUT /api/habits/{id} — update Name/Description (add an UpdateHabitRequest record).

        // TODO(you): DELETE /api/habits/{id} — return Results.NoContent().
    }
}
