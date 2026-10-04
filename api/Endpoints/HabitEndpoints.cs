using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

// Request/response shapes. `record` = immutable data class, great for DTOs.
public record CreateHabitRequest(string Name, string? Description);
public record HabitResponse(int Id, string Name, string? Description, DateTime CreatedAt);
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

            return Results.Created($"/api/habits/{habit.Id}", ToResponse(habit));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var habit = await db.Habits.FindAsync(id);

            if (habit is null)
                return Results.NotFound();

            // Return the DTO, not the EF entity, so every endpoint has the same JSON shape.
            return Results.Ok(ToResponse(habit));
        });

        // Three parameters, each bound automatically:
        // `id` from the route, `request` from the JSON body, `db` from DI.
        group.MapPut("/{id:int}", async (int id, UpdateHabitRequest request, AppDbContext db) =>
        {
            var habit = await db.Habits.FindAsync(id);

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
    }

    private static HabitResponse ToResponse(Habit h) =>
        new(h.Id, h.Name, h.Description, h.CreatedAt);
}
