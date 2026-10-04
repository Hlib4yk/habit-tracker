using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

// DbContext = the database session (think Prisma client). Each DbSet is a table.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Habit> Habits => Set<Habit>();

    public DbSet<CheckIn> CheckIns => Set<CheckIn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Habit>()
            .Property(h => h.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<CheckIn>()
            .HasIndex(c => new { c.HabitId, c.Date }).IsUnique();
    }
}
