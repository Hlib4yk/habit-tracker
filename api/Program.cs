using Api.Data;
using Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services (dependency injection container) ---
builder.Services.AddOpenApi();

// Connection string comes from appsettings.Development.json -> "ConnectionStrings:Default".
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Allow the Next.js dev server to call the API from the browser.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// --- Middleware pipeline (order matters, like app.use() in Express) ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// --- Endpoints ---
app.MapHabitEndpoints();

app.Run();
